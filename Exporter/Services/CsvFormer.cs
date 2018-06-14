using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;

using Exporter.Models;
using Exporter.Models.Interfaces;

namespace Exporter.Services
{
    public class CsvFormer : FileFormer, IFormer
    {
        IUnitOfWork unitOfWork;
        int queryId;
        List<string> parameters = null;
        Dictionary<string, string> queryParameters = null;

        public CsvFormer(IUnitOfWork unitOfWork, string queryId, string[] parameters = null)
        {
            this.unitOfWork = unitOfWork;
            this.queryId = int.Parse(queryId);
            if (parameters != null)
                this.parameters = parameters.ToList();
        }

        public void FormDocument()
        {
            string fullQuery = unitOfWork
                .SqlQueries
                .Get(queryId)
                .SqlQueryContent;

            this.queryParameters = FormQueryParametersDictionary(this.parameters);
            List<string> queries = this.SplitQueryIntoPieces(fullQuery);

            this.CreateFile("csv");

            ServerDbContext server;
            DbCommand command;
            DbDataReader reader;
            StreamWriter writer;
            using (server = new ServerDbContext())
            {
                foreach (string query in queries)
                {
                    using (command = server.Database.Connection.CreateCommand())
                    {
                        server.Database.Connection.Open();

                        this.AddParametersToQuery(ref command, query, queryParameters);

                        command.CommandText = query;

                        using (reader = command.ExecuteReader())
                        {
                            using (writer = new StreamWriter(this.FilePath, true, Encoding.UTF8))
                            {
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        string value = reader[i].ToString();
                                        value = this.WrapItem(value);

                                        if (i != (reader.FieldCount - 1))
                                            writer.Write(value + ",");
                                        else
                                            writer.Write(value);
                                    }
                                    writer.WriteLine();
                                }
                                writer.Close();
                            }
                        }
                        server.Database.Connection.Close();
                    }
                }
            }
        }        
    }
}