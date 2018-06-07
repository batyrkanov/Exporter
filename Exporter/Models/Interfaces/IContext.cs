using System.Data.Entity;
using Exporter.Models.Entities;

namespace Exporter.Models.Interfaces
{
    public interface IContext
    {
        IDbSet<Parameter> Parameters { get; }
        IDbSet<SqlQuery> SqlQueries { get; }
        IDbSet<SqlQueryParameter> SqlQueryParameters { get; }

        void SetModified(object entity);
        void Save();
        void DbDispose();
    }
}
