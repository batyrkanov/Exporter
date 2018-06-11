using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;

using Exporter.Models.Entities;
using Exporter.Models.Interfaces;
using System.IO;
using System.Web.Hosting;

namespace Exporter.Models.Repositories
{
    public class OutputTableRepository : IRepository<OutputTable>
    {
        private IContext db;

        public OutputTableRepository(IContext context) { this.db = context; }

        public IEnumerable<OutputTable> GetAll()
        {
            return db.OutputTables.Include(q => q.SqlQuery);
        }

        public OutputTable Get(int id)
        {
            return db.OutputTables.Find(id);
        }
        
        public void Create(OutputTable outputTable)
        {
            db.OutputTables.Add(outputTable);
        }

        public void Update(OutputTable outputTable)
        {
            db.SetModified(outputTable);
        }

        public void Delete(int id)
        {
            OutputTable outputTable = db.OutputTables.Find(id);
            if (outputTable != null)
                db.OutputTables.Remove(outputTable);
        }

        public void RemoveQueryOutputTableIfExists(int queryId, string type)
        {
            OutputTable outputTable = db
                .OutputTables
                .Where(t => t.QueryId == queryId && t.FileType == type)
                .First();

            if (outputTable != null)
            {
                string filepath = Path
                    .Combine(
                    HostingEnvironment.MapPath("~/Files"),
                    outputTable.TableFileName
                    );

                if (File.Exists(filepath))
                    File.Delete(filepath);
            }
        }

        public void BindOutputTableToQuery(int queryId, string filename, string type)
        {
            SqlQuery query = db.SqlQueries.Find(queryId);

            if (query != null)
            {
                OutputTable outputTable = db
                    .OutputTables
                    .Where(t => t.QueryId == queryId && t.FileType == type)
                    .First();

                if (outputTable != null)
                {
                    outputTable.TableFileName = filename;
                    outputTable.UpdatedAt = DateTime.Now;
                    db.SetModified(outputTable);
                }
                else
                {
                    OutputTable newOutputTable = new OutputTable()
                    {
                        TableName = query.SqlQueryName,
                        TableFileName = filename,
                        FileType = type,
                        QueryId = queryId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                }
            }
        }
    }
}