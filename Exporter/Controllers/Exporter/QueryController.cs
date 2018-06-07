using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Common;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using PagedList;

using Exporter.Models.Entities;
using Exporter.ActionFilters;
using Exporter.Models.Interfaces;
using Exporter.Models.UnitOfWork;
using Exporter.Models;
using Exporter.Services;
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
            if (query != null)
            {
                unitOfWork.SqlQueries.Create(query, parameterIds);
                return RedirectToAction("index");
            }

            return View();

        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SqlQuery query = unitOfWork.SqlQueries.Get((int)id);
            if (query == null)
                return HttpNotFound();

            List<Models.Entities.Parameter> queryParameters = unitOfWork
                .Parameters
                .GetQueryParametersByQueryId(query.SqlQueryId)
                .ToList();

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
                unitOfWork
                    .SqlQueries
                    .Edit(query, parameterIds);

                return RedirectToAction("Index");
            }

            return View(query);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Details(int id)
        {
            SqlQuery query = unitOfWork.SqlQueries.Get(id);
            if (query == null)
                return HttpNotFound();

            List<Models.Entities.Parameter> queryParameters = unitOfWork
                .Parameters
                .GetQueryParametersByQueryId(query.SqlQueryId)
                .ToList();

            ViewBag.QueryParameters = queryParameters;

            return View(query);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SqlQuery query = unitOfWork.SqlQueries.Get((int)id);
            if (query == null)
                return HttpNotFound();

            return View(query);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            unitOfWork
                .SqlQueries
                .DeleteById(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Execute(string input, string[] parameters = null)
        {
            Executor executor = new Executor(input, parameters);

            try { executor.Execute(); return PartialView(executor.Result); }
            catch { return PartialView("~/Views/Query/Error.cshtml"); }

        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult FormCsvFile(string queryId, string[] parameters = null)
        {
            CsvFormer csvFormer = new CsvFormer(unitOfWork, queryId, parameters);
            csvFormer.FormDocument();

            return Json(new { fileName = csvFormer.FileName, errorMessage = "Ошибка. Не удалось сформировать файл. Попытайтесь позже." });
        }

        [HttpPost]
        public JsonResult FormExcelFile(string queryId, string[] parameters = null, HttpPostedFileBase xlsFile = null)
        {
            XlsFormer xlsFormer = new XlsFormer(unitOfWork, queryId, parameters, xlsFile);
            xlsFormer.FormDocument();

            return Json(new { fileName = xlsFormer.FileName, errorMessage = "Ошибка. Не удалось сформировать файл. Попытайтесь позже." });
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
    }
}