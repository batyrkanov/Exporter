using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Exporter.ActionFilters
{
    public class DeleteFileAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuted(ResultExecutedContext context)
        {
            context.HttpContext.Response.Flush();

            string file = (context.Result as FilePathResult).FileName;

            System.IO.File.Delete(file);
        }
    }
}