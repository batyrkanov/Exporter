using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Exporter.Services
{
    public class FileFormer
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }

        protected void CreateFile(string extension)
        {
            string FileName = string.Format("{0}.{1}", Guid.NewGuid().ToString(), extension);
            string FilePath = Path.Combine(HostingEnvironment.MapPath("~/Files"), FileName);

            this.FileName = FileName;
            this.FilePath = FilePath;

            File.Create(FilePath).Dispose();
        }

        protected string WrapItem(string value)
        {
            if (value.Contains(","))
                value = "\"" + value + "\"";

            return value;
        }

        protected Dictionary<string, string> FormQueryParametersDictionary(List<string> parameters)
        {
            if (parameters != null && parameters.Count > 0 && !String.IsNullOrEmpty(parameters[0]))
            {
                Dictionary<string, string> parametersDict = new Dictionary<string, string>();
                foreach (string paramsArrayElement in parameters)
                {
                    string[] parts = paramsArrayElement
                        .Split(new string[] { "-xyz-" }, StringSplitOptions.None);
                    parametersDict[parts[0]] = parts[1];
                }

                return parametersDict;
            }

            return null;
        }

        protected List<string> SplitQueryIntoPieces(string query)
        {
            return Regex
                .Split(
                    query,
                    "union all",
                    RegexOptions.IgnoreCase
                ).ToList();
        }

        protected void AddParametersToQuery(ref DbCommand command, string query, Dictionary<string, string> queryParameters)
        {
            if (queryParameters != null)
                foreach (KeyValuePair<string, string> parameter in queryParameters)
                    if (query.Contains(parameter.Key))
                        command.Parameters.Add(new SqlParameter(parameter.Key, parameter.Value));
        }
    }
}