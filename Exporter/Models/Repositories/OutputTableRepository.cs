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
            return db.OutputTables;
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
                .Where(t => t.SqlQueryId == queryId && t.FileType == type)
                .FirstOrDefault();

            if (outputTable != null)
            {
                string filepath = Path
                    .Combine(
                    HostingEnvironment.MapPath("~/Files"),
                    outputTable.FileName
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
                    .Where(t => t.SqlQueryId == queryId && t.FileType == type)
                    .FirstOrDefault();

                if (outputTable != null)
                {
                    outputTable.FileName = filename;
                    outputTable.UpdatedAt = DateTime.Now;
                    db.SetModified(outputTable);
                }
                else
                {
                    OutputTable newOutputTable = new OutputTable()
                    {
                        Name = query.SqlQueryName,
                        FileName = filename,
                        FileType = type,
                        SqlQueryId = queryId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    db.OutputTables.Add(newOutputTable);
                }
                db.Save();
            }
        }

        public OutputTable GetQueryOutputTableByIdAndType(int queryId, string type)
        {
            OutputTable outputTable = db
                .OutputTables
                .Where(t => t.SqlQueryId == queryId && t.FileType == type)
                .FirstOrDefault();

            return outputTable;
        }

        public void RemoveOutputTableByFileNameAndType(string fileName, string type)
        {
            OutputTable outputTable = db
                .OutputTables
                .Where(t => t.FileName == fileName && t.FileType == type)
                .FirstOrDefault();

            if (outputTable != null)
            {
                string filePath = Path
                    .Combine(
                        HostingEnvironment.MapPath("~/Files"),
                        fileName
                    );

                if (File.Exists(filePath))
                    File.Delete(filePath);

                db
                    .OutputTables
                    .Remove(outputTable);
                db.Save();
            }
        }
    }
}