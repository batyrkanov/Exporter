using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Exporter.Models.Contexts;

namespace Exporter
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer<SqlQueryParameterContext>(null);
        }
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //check for the "file is too big" exception if thrown at the IIS level
            if (Response.StatusCode == 404 && Response.SubStatusCode == 13)
            {
                Response.Write("Too big a file"); //just an example
                Response.End();
            }
        }
    }
}
