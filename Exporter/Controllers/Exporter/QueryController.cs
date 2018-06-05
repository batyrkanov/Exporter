using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;
using ExcApp = Microsoft.Office.Interop.Excel;
using Exporter.Models;

using Exporter.Models.Entities;
using Exporter.ActionFilters;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using System.Web;

namespace Exporter.Controllers.Exporter
{
    public class QueryController : Controller
    {
        SqlQueryParameterContext db = new SqlQueryParameterContext();
        ServerDbContext server = new ServerDbContext();

        [Authorize(Roles = "admin")]
        // GET: Query
        public ActionResult Index(int? page, string searching)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<SqlQuery> queries = db.SqlQueries.Where(x=>x.SqlQueryName.Contains(searching) || searching == null).OrderBy(q => q.SqlQueryName).ToPagedList(pageNumber, pageSize);

            return View(queries);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SqlQuery query, string[] parameterIds = null)
        {
            try
            {
                query.SqlQueryCreatedDate = DateTime.Now;
                db.SqlQueries.Add(query);
                db.SaveChanges();

                if (parameterIds != null && parameterIds.Length > 0)
                {
                    int queryId = query.SqlQueryId;
                    BindQueryAndParams(queryId, parameterIds);
                }

                return RedirectToAction("index");
            } catch
            {
                return View();
            }

        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SqlQuery query = db.SqlQueries.Find(id);
            if (query == null)
                return HttpNotFound();

            List<Models.Entities.Parameter> queryParameters = GetQueryParams(query.SqlQueryId);
            ViewBag.QueryParameters = queryParameters;

            return View(query);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SqlQuery query, string[] parameterIds = null)
        {
            if (ModelState.IsValid)
            {
                SqlQuery currentQuery = db.SqlQueries.FirstOrDefault(p => p.SqlQueryId == query.SqlQueryId);

                currentQuery.SqlQueryName = query.SqlQueryName;
                currentQuery.SqlQueryContent = query.SqlQueryContent;

                RemoveRelations(query.SqlQueryId);

                if (parameterIds != null && parameterIds.Length > 0)
                {
                    int queryId = query.SqlQueryId;
                    BindQueryAndParams(queryId, parameterIds);
                }

                db.Entry(currentQuery).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(query);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SqlQuery query = db.SqlQueries.Find(id);
            if (query == null)
                return HttpNotFound();

            List<Models.Entities.Parameter> queryParameters = GetQueryParams(query.SqlQueryId);
            ViewBag.QueryParameters = queryParameters;

            return View(query);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SqlQuery query = db.SqlQueries.Find(id);
            if (query == null)
                return HttpNotFound();

            return View(query);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SqlQuery query = db.SqlQueries.Find(id);

            RemoveQueryParams(query.SqlQueryId);
            db.SqlQueries.Remove(query);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Execute(string input, string[] parameters = null)
        {
            string query = ReplaceQuotes(input);

            using (var server = new ServerDbContext())
            {
                using (var command = server.Database.Connection.CreateCommand())
                {
                    server.Database.Connection.Open();
                    

                    if (parameters != null && parameters.Length > 0)
                    {
                        foreach (string param in parameters)
                        {
                            string[] parts = param.Split(new string[] { "-xyz-" }, StringSplitOptions.None);
                            command.Parameters.Add(new SqlParameter(parts[0], parts[1]));
                        }
                    }

                    command.CommandText = query;

                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            List<Dictionary<string, object>> model = FormViewDataTable(reader).ToList();
                            return PartialView(model);
                        }
                    }
                    catch
                    {
                        return PartialView("~/Views/Query/Error.cshtml");
                    }
                }
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult FormCsvFile(string input, string[] parameters = null)
        {
            string query = ReplaceQuotes(input);

            using (ServerDbContext server = new ServerDbContext())
            {
                using (DbCommand command = server.Database.Connection.CreateCommand())
                {
                    server.Database.Connection.Open();


                    if (parameters != null && parameters.Length > 0)
                    {
                        foreach (string parameter in parameters)
                        {
                            string[] parts = parameter.Split(new string[] { "-xyz-" }, StringSplitOptions.None);
                            command.Parameters.Add(new SqlParameter(parts[0], parts[1]));
                        }
                    }

                    command.CommandText = query;

                    string ext = "csv";

                    string file = string.Format("{0}.{1}", Guid.NewGuid().ToString(), ext);
                    string filename = Path.Combine(Server.MapPath("~/Files"), file);

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        using (StreamWriter fs = new StreamWriter(new FileStream(filename, FileMode.CreateNew), System.Text.Encoding.UTF8))
                        {
                            string separator = ",";

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string name = reader.GetName(i);
                                name = WrapItem(name);

                                if (i != (reader.FieldCount - 1))
                                    fs.Write(name + separator);
                                else
                                    fs.Write(name);
                            }
                            fs.WriteLine();

                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string value = reader[i].ToString();
                                    value = WrapItem(value);

                                    if (i != (reader.FieldCount - 1))
                                        fs.Write(value + separator);
                                    else
                                        fs.Write(value);
                                }
                                fs.WriteLine();
                            }
                        }
                    }
                    return Json(new { fileName = file, errorMessage = "Ошибка. Не удалось сформировать файл. Попытайтесь позже." });
                }
            }
        }

        [HttpPost]
        public JsonResult FormExcelFile(HttpPostedFileBase header, string input, string[] parameters = null)
        {
            if (header != null && header.ContentLength > 0)
            {
                System.IO.Stream fileContent = header.InputStream;
                Console.WriteLine(fileContent);
            }
            string query = ReplaceQuotes(input);
            using (ServerDbContext db = new ServerDbContext())
            {
                using (var command = db.Database.Connection.CreateCommand())
                {
                    db.Database.Connection.Open();

                    if (parameters != null && parameters.Length > 0)
                    {
                        foreach (string param in parameters)
                        {
                            string[] parts = param.Split(new string[] { "-xyz-" }, StringSplitOptions.None);
                            command.Parameters.Add(new SqlParameter(parts[0], parts[1]));
                        }
                    }

                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        string file = string.Format("{0}.xls", Guid.NewGuid().ToString());
                        string filename = Path.Combine(Server.MapPath("~/Files"), file);
                        Application excel = new Application();

                        //if (excel == null)
                        //    return "Ошибка. Не удалось сформировать xls файл";

                        excel.Visible = false;
                        excel.DisplayAlerts = false;
                        Workbook workbook = excel.Workbooks.Add(Type.Missing);

                        Worksheet worksheet = (Worksheet)workbook.ActiveSheet;
                        worksheet.Name = "Данные";
                        worksheet.Cells.Font.Size = 15;

                        int row = 1;
                        int col = 1;
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string name = reader.GetName(i);
                            worksheet.Cells[row, col] = name;
                            col++;
                        }
                        row++;
                        while (reader.Read())
                        {
                            col = 1;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string value = reader[i].ToString();
                                worksheet.Cells[row, col] = value;
                                col++;
                            }
                            row++;
                        }

                        workbook.SaveAs(filename);
                        workbook.Close(true, Type.Missing, Type.Missing);
                        //workbook.Close(true, filename, Type.Missing);
                        excel.Quit();

                        Marshal.ReleaseComObject(worksheet);
                        Marshal.ReleaseComObject(workbook);
                        Marshal.ReleaseComObject(excel);

                        return Json(new { fileName = file, errorMessage = "Ошибка. Не удалось сформировать файл. Попытайтесь позже." });
                    }

                }
            }

        }

        [DeleteFileAttribute]
        [AllowAnonymous]
        public ActionResult GetFile(string file, string type)
        {
            string filepath = Path.Combine(Server.MapPath("~/Files"), file);
            if (type == "csv")
                return File(filepath, "text/csv", file);
            else
                return File(filepath, "application/vnd.ms-excel", file);
        }

        private List<Dictionary<string, object>> FormViewDataTable(DbDataReader reader)
        {
            List<Dictionary<string, object>> expandoList = new List<Dictionary<string, object>>();
            foreach (var item in reader)
            {
                IDictionary<string, object> expando = new ExpandoObject();
                foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(item))
                {
                    var obj = propertyDescriptor.GetValue(item);
                    expando.Add(propertyDescriptor.Name, obj);
                }
                expandoList.Add(new Dictionary<string, object>(expando));
            }
            return expandoList;
        }

        private void BindQueryAndParams(int queryId, string[] parameters)
        {
            foreach (string param in parameters)
            {
                SqlQueryParameter item = new SqlQueryParameter();
                item.SqlQueryId = queryId;
                item.ParameterId = int.Parse(param);
                db.SqlQueriesParameters.Add(item);
                db.SaveChanges();
            }
        }

        [Authorize(Roles = "admin")]
        private void RemoveQueryParams(int queryId)
        {
            List<int> queryParameterIds = db.SqlQueriesParameters.Where(q => q.SqlQueryId == queryId).Select(i => i.ParameterId).ToList();
            db.SqlQueriesParameters.RemoveRange(db.SqlQueriesParameters.Where(q => q.SqlQueryId == queryId));
            
            if (queryParameterIds != null && !(queryParameterIds.Count <= 0))
                db.Parameters.RemoveRange(db.Parameters.Where(p => queryParameterIds.Contains(p.ParameterId)));
        }

        [Authorize(Roles = "admin")]
        private void RemoveRelations(int queryId)
        {
            db.SqlQueriesParameters.RemoveRange(db.SqlQueriesParameters.Where(q => q.SqlQueryId == queryId));
        }

        private List<Models.Entities.Parameter> GetQueryParams(int queryId)
        {
            List<int> paramIds = db.SqlQueriesParameters.Where(q => q.SqlQueryId == queryId).Select(i => i.ParameterId).ToList();
            List<Models.Entities.Parameter> parameters = db.Parameters.Where(p => paramIds.Contains(p.ParameterId)).ToList();

            return parameters;
        }

        private string ReplaceQuotes(string query)
        {
            string output = query.Replace("\"", "'");
            return output;
        }

        private string WrapItem(string value)
        {
            if (value.Contains(","))
                value = "\"" + value + "\"";

            return value;
        }
    }
}