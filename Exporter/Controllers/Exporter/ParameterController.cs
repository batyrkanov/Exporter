using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;

using Exporter.Models.Interfaces;
using Exporter.Models.Entities;
using Exporter.Models.UnitOfWork;
using Const = Exporter.Constants;

namespace Exporter.Controllers.Exporter
{
    public class ParameterController : Controller
    {
        IUnitOfWork unitOfWork;

        public ParameterController()
        {
            this.unitOfWork = new UnitOfWork();
        }

        public ParameterController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "admin")]
        public ActionResult Index(int? page, string searching)

        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<Parameter> parameters = unitOfWork
                .Parameters
                .FindParametersByNameOrderedByCreatedDesc(searching)
                .ToPagedList(pageNumber, pageSize);
            return View(parameters);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            ViewBag.Types = Const.Constant.InputTypes;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Parameter parameter, string ParameterType)
        {
            if (parameter != null)
            {
                string type = Const.Constant.InputTypes[int.Parse(ParameterType)];
                unitOfWork
                    .Parameters
                    .Create(parameter, type);
                return RedirectToAction("Index");
            }

            ViewBag.Types = Const.Constant.InputTypes;
            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Parameter parameter = unitOfWork.Parameters.Get((int)id);
            if (parameter == null)
                return HttpNotFound();

            ViewBag.Types = Const.Constant.InputTypes;
            return View(parameter);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Parameter parameter, string ParameterType) // parameterType = "3"(string)
        {
            if (ModelState.IsValid)
            {
                string type = Const.Constant.InputTypes[int.Parse(ParameterType)];
                unitOfWork.Parameters.Update(parameter, type);

                return RedirectToAction("Index");
            }
            return View(parameter);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Parameter parameter = unitOfWork.Parameters.Get((int)id);
            if (parameter == null)
                return HttpNotFound();

            List<SqlQuery> queries = GetParameterQueries(parameter.ParameterId);
            ViewBag.Queries = queries;

            return View(parameter);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Parameter parameter = unitOfWork.Parameters.Get((int)id);
            if (parameter == null)
                return HttpNotFound();

            List<SqlQuery> queries = GetParameterQueries(parameter.ParameterId);
            ViewBag.Queries = queries;

            return View(parameter);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Parameter parameter = unitOfWork.Parameters.Get((int)id);
            if (parameter == null)
                return HttpNotFound();

            List<SqlQuery> queries = GetParameterQueries(parameter.ParameterId);
            if (!(queries.Count <= 0))
                return View("~/Views/Parameter/Aborted.cshtml");

            unitOfWork.Parameters.Delete(parameter.ParameterId);
            unitOfWork.Save();

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult CreateForm()
        {
            ViewBag.Types = Const.Constant.InputTypes;
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public int CreateParameter(string name, string ruName, int type)
        {
            int parameterId = unitOfWork
                .Parameters
                .Create(name, ruName, Const.Constant.InputTypes[type]);

            return parameterId;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditParameter(int parameterId)
        {
            Parameter parameter = unitOfWork
                .Parameters
                .Get(parameterId);
            string parentId = String.Format("id-{0}", parameter.ParameterName.Replace("@", ""));

            ViewBag.ParentId = parentId;
            ViewBag.Types = Const.Constant.InputTypes;

            return PartialView(parameter);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public int SaveChanges(int parameterId, string name, string ruName, int type)
        {
            if (ModelState.IsValid)
            {
                string typeName = Const.Constant.InputTypes[type];
                int paramId = unitOfWork
                    .Parameters
                    .SaveChanges(parameterId, name, ruName, typeName);

                return paramId;
            }
            return parameterId;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public string RemoveParameter(int parameterId)
        {
            Parameter parameter = unitOfWork.Parameters.Get(parameterId);

            string id = String.Format("id-{0}", parameter.ParameterName).Replace("@", "");

            unitOfWork.Parameters.Delete(parameter.ParameterId);
            unitOfWork.Save();

            return id;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GenerateInput(int? parameterId)
        {
            if (parameterId != null)
            {
                Parameter parameter = unitOfWork.Parameters.Get((int)parameterId);
                string name = parameter.ParameterName.Replace("@", "");
                ViewBag.ParameterNameId = String.Format("id-{0}", name);
                ViewBag.BtnEditId = String.Format("edit-{0}", name);
                ViewBag.BtnRemoveId = String.Format("remove-{0}", name);

                return PartialView(parameter);
            }

            return PartialView("~/Views/Query/Error.cshtml");
        }

        [AllowAnonymous]
        private List<SqlQuery> GetParameterQueries(int parameterId)
        {
            List<int> queryIds = unitOfWork
                .SqlQueryParameters
                .GetSqlQueryIdByParameterId(parameterId)
                .ToList();

            List<SqlQuery> queries = unitOfWork
                .SqlQueries
                .GetQueriesFromListById(queryIds)
                .ToList();

            return queries;
        }
    }
}