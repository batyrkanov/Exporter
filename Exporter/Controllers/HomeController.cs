using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Exporter.Models.Entities;
using PagedList;
using Exporter.Models;

using Exporter.Models.Interfaces;
using Exporter.Models.UnitOfWork;
using Const = Exporter.Constants;

namespace Exporter.Controllers
{
    public class HomeController : Controller
    {
        ServerDbContext server = new ServerDbContext();

        IUnitOfWork unitOfWork;
        public HomeController()
        {
            this.unitOfWork = new UnitOfWork();
        }
        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Queries(int? page, string searching)
        {
            int pageSize = Const.Constant.NumberOfQueriesPerPage;
            int pageNumber = (page ?? 1);

            IPagedList<SqlQuery> queries = unitOfWork
                .SqlQueries
                .FindQueriesByNameOrderByName(searching)
                .ToPagedList(pageNumber, pageSize);

            return View(queries);
        }

        [AllowAnonymous]
        public ActionResult Unloading(int id)
        {
            IQueryable<Parameter> parameters = unitOfWork
                .Parameters
                .GetQueryParametersByQueryId(id)
                .AsQueryable();

            ViewBag.Parameters = parameters;
            ViewBag.QueryId = id;

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
            List<SqlQuery> queries = unitOfWork
                .SqlQueries
                .GetQueriesById(testID)
                .ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                queries = unitOfWork
                    .SqlQueries
                    .GetQueriesFromListByName(queries, searchString)
                    .ToList();
            }

            //PageSize displays the maximum number of rows on each page
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            queries = unitOfWork
                .SqlQueries
                .OrderQueryByNameDesc(queries).ToList();

            return PartialView("QuerySearch", queries.ToPagedList(pageNumber, pageSize));
        }
    }
}
