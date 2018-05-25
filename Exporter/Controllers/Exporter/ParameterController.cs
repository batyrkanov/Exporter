using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Exporter.Models.Entities;
using PagedList.Mvc;
using PagedList;

namespace Exporter.Controllers.Exporter
{
    public class ParameterController : Controller
    {
        public static readonly Dictionary<int, string> InputTypes = new Dictionary<int, string>() { { 1, "text" }, { 2, "number" }, { 3, "date" }, { 4, "time" }, { 5, "week" }, { 6, "month" }, { 7, "email" }, { 8, "tel" } };

        SqlQueryParameterContext db = new SqlQueryParameterContext();
        // GET: Parameter

        [Authorize(Roles = "admin")]
        public ActionResult Index(int? page, string searching)

        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<Parameter> parameters = db.Parameters.Where(x=>x.ParameterName.Contains(searching) || searching == null).OrderByDescending(p => p.ParameterCreatedDate).ToPagedList(pageNumber, pageSize);
            return View(parameters);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            ViewBag.Types = InputTypes;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Parameter parameter, string ParameterType)
        {
            try
            {
                parameter.ParameterCreatedDate = DateTime.Now;
                parameter.ParameterType = InputTypes[int.Parse(ParameterType)];
                db.Parameters.Add(parameter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.Types = InputTypes;
                return View();
            }
        }

        [Authorize(Roles = "admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Parameter parameter = db.Parameters.Find(id);
            if (parameter == null)
                return HttpNotFound();

            ViewBag.Types = InputTypes;
            return View(parameter);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Parameter parameter, string ParameterType)
        {
            if (ModelState.IsValid)
            {
                Parameter currentParameter = db.Parameters.FirstOrDefault(p => p.ParameterId == parameter.ParameterId);
                var type = parameter.ParameterType = InputTypes[int.Parse(ParameterType)];
                currentParameter.ParameterName = parameter.ParameterName;
                currentParameter.ParameterRuName = parameter.ParameterRuName;
                currentParameter.ParameterType = type;

                db.Entry(currentParameter).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(parameter);
        }

        [Authorize(Roles = "admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Parameter parameter = db.Parameters.Find(id);
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

            Parameter parameter = db.Parameters.Find(id);
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

            Parameter parameter = db.Parameters.Find(id);
            if (parameter == null)
                return HttpNotFound();

            List<SqlQuery> queries = GetParameterQueries(parameter.ParameterId);
            if (!(queries.Count <= 0))
                return View("~/Views/Parameter/Aborted.cshtml");

            db.Parameters.Remove(parameter);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult CreateForm()
        {
            ViewBag.Types = InputTypes;
            return PartialView();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public int CreateParameter(string name, string ruName, int type)
        {
            Parameter parameter = new Parameter
            {
                ParameterName = name,
                ParameterRuName = ruName,
                ParameterType = InputTypes[type],
                ParameterCreatedDate = DateTime.Now
            };
            db.Parameters.Add(parameter);
            db.SaveChanges();

            return parameter.ParameterId;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult EditParameter(int parameterId)
        {
            Parameter parameter = db.Parameters.Find(parameterId);
            string parentId = String.Format("id-{0}", parameter.ParameterName.Replace("@", ""));
            ViewBag.ParentId = parentId;
            ViewBag.Types = InputTypes;

            return PartialView(parameter);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public int SaveChanges(int parameterId, string name, string ruName, int type)
        {
            if (ModelState.IsValid)
            {
                Parameter parameter = db.Parameters.Find(parameterId);

                parameter.ParameterName = name;
                parameter.ParameterRuName = ruName;
                parameter.ParameterType = InputTypes[type];
                db.Entry(parameter).State = EntityState.Modified;
                db.SaveChanges();

                //try
                //{
                //    db.Entry(parameter).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
                //catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                //{
                //    foreach (var errors in ex.EntityValidationErrors)
                //    {
                //        foreach (var error in errors.ValidationErrors)
                //        {
                //            Console.WriteLine("Property: {0}, Error: {1}", error.PropertyName, error.ErrorMessage);
                //        }
                //    }
                //}

                return parameter.ParameterId;
            }
            return parameterId;
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public string RemoveParameter(int parameterId)
        {
            Parameter parameter = db.Parameters.Find(parameterId);

            string id = String.Format("id-{0}", parameter.ParameterName).Replace("@", "");

            db.Parameters.Remove(parameter);
            db.SaveChanges();

            return id;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GenerateInput(int? parameterId)
        {
            if (parameterId != null)
            {
                Parameter parameter = db.Parameters.Find(parameterId);
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
            List<int> queryIds = db.SqlQueriesParameters.Where(s => s.ParameterId == parameterId).Select(i => i.SqlQueryId).ToList();
            List<SqlQuery> queries = db.SqlQueries.Where(q => queryIds.Contains(q.SqlQueryId)).ToList();

            return queries;
        }
    }
}