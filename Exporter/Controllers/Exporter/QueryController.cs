using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

using PagedList;
using Excel = Microsoft.Office.Interop.Excel;

using Exporter.Models.Entities;
using Exporter.ActionFilters;
using Exporter.Models.Interfaces;
using Exporter.Models.UnitOfWork;
using Exporter.Models;
using Const = Exporter.Constants;

namespace Exporter.Controllers.Exporter
{
    public class QueryController : Controller
    {
        IUnitOfWork unitOfWork;
        readonly ServerDbContext server = new ServerDbContext();

        public QueryController()
        {
            this.unitOfWork = new UnitOfWork();
        }
        public QueryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "admin")]
        // GET: Query
        public ActionResult Index(int? page, string searching)
        {
            int pageSize = Const.Constant.NumberOfQueriesPerPage;
            int pageNumber = (page ?? 1);

            IPagedList<SqlQuery> queries = unitOfWork
                .SqlQueries
                .FindQueriesByNameOrderByName(searching)
                .ToPagedList(pageNumber, pageSize);

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
            }
            catch
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
        public JsonResult FormCsvFile(string queryId, string[] parameters = null)
        {
            string fullQuery = db.SqlQueries.Find(int.Parse(queryId)).SqlQueryContent;
            Dictionary<string, string> queryParameters = null;
            if (parameters != null && parameters.Length > 0 && !String.IsNullOrEmpty(parameters[0]))
                queryParameters = FormQueryParametersDictionary(parameters);

            List<string> queries = Regex.Split(fullQuery, "union all", RegexOptions.IgnoreCase).ToList();

            string[] fileData = CreateFileAndGetFileNameAndPath("csv");
            string csvFileName = fileData.First();
            string csvFilePath = fileData.Last();
            using (ServerDbContext server = new ServerDbContext())
            {
                foreach (string query in queries)
                {
                    using (DbCommand command = server.Database.Connection.CreateCommand())
                    {
                        server.Database.Connection.Open();

                        if (queryParameters != null)
                            foreach (KeyValuePair<string, string> parameter in queryParameters)
                                if (query.Contains(parameter.Key))
                                    command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));

                        command.CommandText = query;

                        using (DbDataReader reader = command.ExecuteReader())
                        {
                            using (StreamWriter csvFile = new StreamWriter(csvFilePath, true, Encoding.UTF8))
                            {
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        string value = reader[i].ToString();
                                        value = WrapItem(value);

                                        if (i != (reader.FieldCount - 1))
                                            csvFile.Write(value + ",");
                                        else
                                            csvFile.Write(value);
                                    }
                                    csvFile.WriteLine();
                                }
                                csvFile.Close();
                            }
                        }

                        server.Database.Connection.Close();
                    }
                }
            }

            return Json(new { fileName = csvFileName, errorMessage = "Ошибка. Не удалось сформировать файл. Попытайтесь позже." });
        }

        [HttpPost]
        public JsonResult FormExcelFile(string queryId, string[] parameters = null, HttpPostedFileBase xlsFile = null)
        {
            // save file or create if uploaded file does not exist
            string[] fileData = CreateFileAndGetFileNameAndPath("xls");
            string xlsFileName = fileData.First();
            string xlsFilePath = fileData.Last();

            if (xlsFile != null && xlsFile.ContentLength > 0)
            {
                xlsFile.SaveAs(xlsFilePath);
            }
            else
            {
                Excel.Application app = new Excel.Application()
                {
                    Visible = false,
                    DisplayAlerts = false
                };
                Excel.Workbook book = app.Workbooks.Add(Type.Missing);
                Excel.Worksheet sheet = (Excel.Worksheet)book.ActiveSheet;
                sheet.Name = "Data";
                sheet.Cells.Font.Size = 14;

                book.SaveAs(xlsFilePath);
                book.Close(true, Type.Missing, Type.Missing);
                app.Quit();

                Marshal.ReleaseComObject(sheet);
                Marshal.ReleaseComObject(book);
                Marshal.ReleaseComObject(app);
            }

            // Разбиваем запрос на мелкие запросы
            string fullQuery = db.SqlQueries.Find(int.Parse(queryId)).SqlQueryContent;
            Dictionary<string, string> queryParameters = null;
            if (parameters != null && parameters.Length > 0 && !String.IsNullOrEmpty(parameters[0]))
                queryParameters = FormQueryParametersDictionary(parameters);

            List<string> queries = Regex.Split(fullQuery, "union all", RegexOptions.IgnoreCase).ToList();

            Excel.Application xlsApp;
            Excel.Workbook workbook;
            Excel.Worksheet worksheet;
            int? row = null;
            using (ServerDbContext db = new ServerDbContext())
            {
                foreach (string query in queries)
                {
                    using (DbCommand command = db.Database.Connection.CreateCommand())
                    {
                        db.Database.Connection.Open();

                        if (queryParameters != null)
                            foreach (KeyValuePair<string, string> parameter in queryParameters)
                                if (query.Contains(parameter.Key))
                                    command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));

                        command.CommandText = query;
                        using (var reader = command.ExecuteReader())
                        {
                            xlsApp = new Excel.Application();
                            workbook = xlsApp.Workbooks.Open(xlsFilePath);
                            worksheet = (Excel.Worksheet)workbook.Worksheets[1];
                            if (row == null)
                            {
                                object misValue = Missing.Value;
                                row = 0;
                                Excel.Range count = worksheet.UsedRange.Columns[1, misValue] as Excel.Range;

                                foreach (Excel.Range cell in count.Cells)
                                    row++;
                            }

                            while (reader.Read())
                            {
                                row++;
                                int col = 1;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string value = reader[i].ToString();
                                    worksheet.Cells[row, col] = value;
                                    col++;
                                }
                            }

                            workbook.Save();
                            workbook.Close();
                            xlsApp.Quit();

                            Marshal.ReleaseComObject(worksheet);
                            Marshal.ReleaseComObject(workbook);
                            Marshal.ReleaseComObject(xlsApp);
                        }

                        db.Database.Connection.Close();
                    }
                }
                return Json(new { fileName = xlsFileName, errorMessage = "Ошибка. Не удалось сформировать файл. Попытайтесь позже." });
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
                SqlQueryParameter item = new SqlQueryParameter()
                {
                    SqlQueryId = queryId,
                    ParameterId = int.Parse(param)
                };
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

        private Dictionary<string, string> FormQueryParametersDictionary(string[] paramsArray)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            foreach (string paramsArrayElement in paramsArray)
            {
                string[] parts = paramsArrayElement.Split(new string[] { "-xyz-" }, StringSplitOptions.None);
                parameters[parts[0]] = parts[1];
            }

            return parameters;
        }

        private string[] CreateFileAndGetFileNameAndPath(string extension)
        {
            string fileName = string.Format("{0}.{1}", Guid.NewGuid().ToString(), extension);
            string filePath = Path.Combine(Server.MapPath("~/Files"), fileName);
            System.IO.File.Create(filePath).Dispose();

            return new string[2] { fileName, filePath };
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