using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Exporter.Models.Entities;
using Exporter.Models.Interfaces;

namespace Exporter.Models.Repositories
{
    public class ParameterRepository : IRepository<Parameter>
    {
        private IContext db;

        public ParameterRepository(IContext context)
        {
            this.db = context;
        }

        public IEnumerable<Parameter> GetAll()
        {
            return db.Parameters.Include(p => p.SqlQueriesParameters);
        }

        public Parameter Get(int id)
        {
            return db.Parameters.Find(id);
        }

        public void Create(Parameter parameter)
        {
            db.Parameters.Add(parameter);
        }

        public void Update(Parameter parameter)
        {
            db.SetModified(parameter);
        }

        public void Delete(int id)
        {
            Parameter parameter = db.Parameters.Find(id);
            if (parameter != null)
                db.Parameters.Remove(parameter);
        }

        public int SaveChanges(int parameterId, string name, string ruName, string type)
        {
            Parameter parameter = db.Parameters.Find(parameterId);
            if (parameter != null)
            {
                parameter.ParameterName = name;
                parameter.ParameterRuName = ruName;
                parameter.ParameterType = type;

                db.SetModified(parameter);
                db.Save();

                return parameter.ParameterId;
            }
            return parameterId;
        }

        public void Create(Parameter parameter, string type)
        {
            parameter.ParameterCreatedDate = DateTime.Now;
            parameter.ParameterType = type;

            db.Parameters.Add(parameter);
            db.Save();
        }

        public int Create(string name, string ruName, string type)
        {
            Parameter parameter = new Parameter
            {
                ParameterName = name,
                ParameterRuName = ruName,
                ParameterType = type,
                ParameterCreatedDate = DateTime.Now
            };
            db.Parameters.Add(parameter);
            db.Save();

            return parameter.ParameterId;
        }

        public void Update(Parameter inputParameter, string type)
        {
            Parameter parameter = db.Parameters.Find(inputParameter.ParameterId);
            if (parameter != null)
            {
                parameter.ParameterName = inputParameter.ParameterName;
                parameter.ParameterRuName = inputParameter.ParameterRuName;
                parameter.ParameterType = type;

                db.SetModified(parameter);
                db.Save();
            }
        }

        public IEnumerable<Parameter> GetQueryParametersByQueryId(int queryId)
        {
            List<int> paramIds = db
                .SqlQueryParameters
                .Where(q => q.SqlQueryId == queryId)
                .Select(i => i.ParameterId)
                .ToList();
            IEnumerable<Parameter> parameters = db
                .Parameters
                .Where(p => paramIds.Contains(p.ParameterId));

            return parameters;
        }

        public IEnumerable<Parameter> FindParametersByNameOrderedByCreatedDesc(string name)
        {
            IEnumerable<Parameter> parameters = db.Parameters.Where(x => x.ParameterName.Contains(name) || name == null).OrderByDescending(p => p.ParameterCreatedDate);
            return parameters;
        }
    }
}