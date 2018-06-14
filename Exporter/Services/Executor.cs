using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Dynamic;
using System.Linq;

using Exporter.Models;
using Exporter.Models.Interfaces;

namespace Exporter.Services
{
    public class Executor : FileFormer, IExecutor
    {
        string query;
        List<string> parameters = null;
        Dictionary<string, string> queryParameters = null;
        public List<Dictionary<string, object>> Result { get; set; }

        public Executor(string query, string[] parameters = null)
        {
            this.query = this.ReplaceQuotes(query);
            if (parameters != null)
                this.parameters = parameters.ToList();
        }

        public void Execute()
        {
            this.queryParameters = FormQueryParametersDictionary(this.parameters);

            ServerDbContext server;
            DbCommand command;
            DbDataReader reader;
            using (server = new ServerDbContext())
            {
                using (command = server.Database.Connection.CreateCommand())
                {
                    server.Database.Connection.Open();

                    this.AddParametersToQuery(ref command, query, this.queryParameters);

                    command.CommandText = query;

                    using (reader = command.ExecuteReader())
                    {
                        this.Result = this.FormViewDataTable(reader).ToList();
                    }
                }
            }

        }

        private List<Dictionary<string, object>> FormViewDataTable(DbDataReader reader)
        {
            List<Dictionary<string, object>> expandoList = new List<Dictionary<string, object>>();

            foreach (object item in reader)
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

        private string ReplaceQuotes(string query)
        {
            string output = query.Replace("\"", "'");
            return output;
        }
    }
}