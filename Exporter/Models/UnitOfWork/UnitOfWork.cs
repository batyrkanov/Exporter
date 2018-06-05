using Exporter.Models.Interfaces;
using Exporter.Models.Repositories;
using Exporter.Models.Entities;
using Exporter.Models.Contexts;
using System;

namespace Exporter.Models.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private IContext db;

        private ParameterRepository parameterRepository;
        private QueryRepository queryRepository;
        private SqlQueryParameterRepository sqlQueryParameterRepository;
        

        public UnitOfWork()
        {
            db = new SqlQueryParameterContext();
        }

        public UnitOfWork(string connectionString)
        {
            db = new SqlQueryParameterContext(connectionString);
        }

        public ParameterRepository Parameters
        {
            get
            {
                if (parameterRepository == null)
                    parameterRepository = new ParameterRepository(db);
                return parameterRepository;
            }
        }

        public QueryRepository SqlQueries
        {
            get
            {
                if (queryRepository == null)
                    queryRepository = new QueryRepository(db);
                return queryRepository;
            }
        }

        public SqlQueryParameterRepository SqlQueryParameters
        {
            get
            {
                if (sqlQueryParameterRepository == null)
                    sqlQueryParameterRepository = new SqlQueryParameterRepository(db);
                return sqlQueryParameterRepository;
            }
        }

        public void Save()
        {
            db.Save();
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.DbDispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}