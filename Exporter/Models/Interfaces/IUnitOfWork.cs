using System;
using Exporter.Models.Entities;
using Exporter.Models.Repositories;

namespace Exporter.Models.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ParameterRepository Parameters { get; }
        QueryRepository SqlQueries { get; }
        SqlQueryParameterRepository SqlQueryParameters { get; }
        OutputTableRepository OutputTables { get; }
        void Save();
    }
}
