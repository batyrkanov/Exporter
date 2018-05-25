using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Exporter.Models.Entities;
using PagedList.Mvc;
using PagedList;
using Exporter.Models;
namespace Exporter.Controllers
{
    public class HomeController : Controller
    {
        SqlQueryParameterContext db = new SqlQueryParameterContext();
        ServerDbContext server = new ServerDbContext();

        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult Queries(int? page, string searching)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<SqlQuery> queries = db.SqlQueries.OrderBy(q => q.SqlQueryName).Where(x=>x.SqlQueryName.Contains(searching) || searching == null).ToPagedList(pageNumber, pageSize);

            return View(queries);
        }

        [AllowAnonymous]
        public ActionResult Unloading(int? id)
        {
            if (id == null)
                return HttpNotFound();

            List<int> ids = db.SqlQueriesParameters.Where(s => s.SqlQueryId == id).Select(i => i.ParameterId).ToList();
            IQueryable<Parameter> parameters = db.Parameters.Where(p => ids.Contains(p.ParameterId));
            string query = db.SqlQueries.Find(id).SqlQueryContent;

            ViewBag.Parameters = parameters;
            ViewBag.Query = query;

            return View();
        }
        
        public PartialViewResult QuerySearch(string searchString, string currentFilter, int? page, string sortOrder)
        {
            ViewBag.CurrentSort = sortOrder;

            string myID = "0";
            if (Session["myID"] != null)
            {
                myID = Session["myID"].ToString();
            }
            int testID;
            try
            {
                testID = Convert.ToInt32(myID);
            }
            catch (FormatException ex)
            {
                throw new FormatException("Ошибка", ex);
            }

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            //Linq code that gets data from the database
            var message = (from query in db.SqlQueries
                           where query.SqlQueryId == testID
                           select query);

            if (!String.IsNullOrEmpty(searchString))
            {
                message = message.Where(s => s.SqlQueryName.Contains(searchString));
            }

            //PageSize displays the maximum number of rows on each page
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return PartialView("QuerySearch", message.OrderByDescending(i => i.SqlQueryName).ToPagedList(pageNumber, pageSize));
        }
    }
}
