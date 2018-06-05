using System.Data.Entity;
using Exporter.Models.Entities;

namespace Exporter.Models.Interfaces
{
    public interface IContext
    {
        DbSet<Parameter> Parameters { get; }
        DbSet<SqlQuery> SqlQueries { get; }
        DbSet<SqlQueryParameter> SqlQueryParameters { get; }

        void SetModified(object entity);
        void Save();
        void DbDispose();
    }
}
