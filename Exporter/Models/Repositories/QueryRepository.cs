using System;
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

        public void Create(SqlQuery query, string[] parameterIds = null)
        {
            query.SqlQueryCreatedDate = DateTime.Now;
            db.SqlQueries.Add(query);
            db.Save();

            if (parameterIds != null && parameterIds.Length > 0)
            {
                int queryId = query.SqlQueryId;
                BindQueryAndParams(queryId, parameterIds);
            }
        }

        public void Edit(SqlQuery inputQuery, string[] parameterIds = null)
        {
            SqlQuery query = db.SqlQueries.Find(inputQuery.SqlQueryId);
            query.SqlQueryName = inputQuery.SqlQueryName;
            query.SqlQueryContent = inputQuery.SqlQueryContent;

            RemoveParametersRelationsFromQuery(query.SqlQueryId);

            if (parameterIds != null && parameterIds.Length > 0)
            {
                int queryId = query.SqlQueryId;
                BindQueryAndParams(queryId, parameterIds);
            }

            db.SetModified(query);
            db.Save();
        }

        public void DeleteById(int id)
        {
            SqlQuery query = db.SqlQueries.Find(id);
            if (query != null)
            {
                RemoveQueryParameters(id);
                db.SqlQueries.Remove(query);
                db.Save();
            }
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

        public IEnumerable<SqlQuery> GetQueriesById(int id)
        {
            return (from query in db.SqlQueries
                    where query.SqlQueryId == id
                    select query);
        }

        public IEnumerable<SqlQuery> GetQueriesFromListByName(List<SqlQuery> queries, string name)
        {
            return queries
                .Where(q => q.SqlQueryName.Contains(name));
        }

        public IEnumerable<SqlQuery> OrderQueryByNameDesc(List<SqlQuery> queries)
        {
            return queries
                .OrderByDescending(q => q.SqlQueryName);
        }

        private void RemoveParametersRelationsFromQuery(int queryId)
        {
            IEnumerable<SqlQueryParameter> list = db
                .SqlQueryParameters
                .Where(q => q.SqlQueryId == queryId);
            this.RemoveSqlQueryParametersRange(list);
        }

        private void RemoveQueryParameters(int queryId)
        {
            List<int> queryParameterIds = db
                .SqlQueryParameters
                .Where(q => q.SqlQueryId == queryId)
                .Select(i => i.ParameterId)
                .ToList();
            RemoveParametersRelationsFromQuery(queryId);

            if (queryParameterIds != null && !(queryParameterIds.Count <= 0))
            {
                IEnumerable<Parameter> parameters = db
                    .Parameters
                    .Where(p => queryParameterIds.Contains(p.ParameterId));
                this.RemoveParameterRange(parameters);
            }
        }

        private void RemoveSqlQueryParametersRange(IEnumerable<SqlQueryParameter> queries)
        {
            for (int i = queries.Count() - 1; i >= 0; i--)
            {
                SqlQueryParameter query = queries.ElementAt(i);
                if (db.SqlQueryParameters.Contains(query))
                    db.SqlQueryParameters.Remove(query);
            }
        }

        private void RemoveParameterRange(IEnumerable<Parameter> parameters)
        {
            for (int i = parameters.Count() - 1; i >= 0; i--)
            {
                Parameter parameter = parameters.ElementAt(i);
                if (db.Parameters.Contains(parameter))
                    db.Parameters.Remove(parameter);
            }
        }

        private void BindQueryAndParams(int queryId, string[] parameters)
        {
            foreach (string param in parameters)
            {
                SqlQueryParameter item = new SqlQueryParameter()
                {
                    SqlQueryId = queryId,
                    ParameterId = int.Parse(param)
                };

                db.SqlQueryParameters.Add(item);
                db.Save();
            }
        }
    }
}