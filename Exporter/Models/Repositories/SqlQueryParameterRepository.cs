using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Exporter.Models.Entities;
using Exporter.Models.Interfaces;

namespace Exporter.Models.Repositories
{
    public class SqlQueryParameterRepository : IRepository<SqlQueryParameter>
    {
        private IContext db;

        public SqlQueryParameterRepository(IContext context)
        {
            this.db = context;
        }

        public IEnumerable<SqlQueryParameter> GetAll()
        {
            return db.SqlQueryParameters.Include(p => p.SqlQuery);
        }

        public SqlQueryParameter Get(int id)
        {
            return db.SqlQueryParameters.Find(id);
        }

        public void Create(SqlQueryParameter sqlQueryParameter)
        {
            db.SqlQueryParameters.Add(sqlQueryParameter);
        }

        public void Update(SqlQueryParameter sqlQueryParameter)
        {
            db.SetModified(sqlQueryParameter);
        }

        public void Delete(int id)
        {
            SqlQueryParameter sqlQueryParameter = db.SqlQueryParameters.Find(id);
            if (sqlQueryParameter != null)
                db.SqlQueryParameters.Remove(sqlQueryParameter);
        }

        public IEnumerable<int> GetSqlQueryIdByParameterId(int parameterId)
        {
            IEnumerable<int> queryIds = db
                .SqlQueryParameters
                .Where(s => s.ParameterId == parameterId)
                .Select(i => i.SqlQueryId);

            return queryIds;
        }
    }
}