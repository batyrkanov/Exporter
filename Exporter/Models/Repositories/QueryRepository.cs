using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Exporter.Models.Entities;
using Exporter.Models.Interfaces;
using Const = Exporter.Constants;

namespace Exporter.Models.Repositories
{
    public class QueryRepository : IRepository<SqlQuery>
    {
        private IContext db;

        public QueryRepository(IContext context)
        {
            this.db = context;
        }

        public IEnumerable<SqlQuery> GetAll()
        {
            return db.SqlQueries.Include(q => q.SqlQueryParameters);
        }

        public SqlQuery Get(int id)
        {
            return db.SqlQueries.Find(id);
        }

        public void Create(SqlQuery query)
        {
            db.SqlQueries.Add(query);
        }

        public void Update(SqlQuery query)
        {
            db.SetModified(query);
        }

        public void Delete(int id)
        {
            SqlQuery query = db.SqlQueries.Find(id);
            if (query != null)
                db.SqlQueries.Remove(query);
        }

        public IEnumerable<SqlQuery> GetQueriesFromListById(List<int> identifiers)
        {
            IEnumerable<SqlQuery> queries = db
                .SqlQueries
                .Where(q => identifiers.Contains(q.SqlQueryId));

            return queries;
        }

        public IEnumerable<SqlQuery> FindQueriesByNameOrderByName(string name)
        {
            IEnumerable<SqlQuery> queries = db.SqlQueries
                .Where(q => q.SqlQueryName.Contains(name) || name == null)
                .OrderBy(q => q.SqlQueryName);

            return queries;
        }
    }
}