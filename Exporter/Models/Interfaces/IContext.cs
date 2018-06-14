using System.Data.Entity;
using Exporter.Models.Entities;

namespace Exporter.Models.Interfaces
{
    public interface IContext
    {
        IDbSet<Parameter> Parameters { get; }
        IDbSet<SqlQuery> SqlQueries { get; }
        IDbSet<SqlQueryParameter> SqlQueryParameters { get; }
        IDbSet<OutputTable> OutputTables { get; }

        void SetModified(object entity);
        void Save();
        void DbDispose();
    }
}
