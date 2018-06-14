using System.Data.Entity;
using Exporter.Models.Interfaces;
using Exporter.Models.Entities;

namespace Exporter.Models.Contexts
{
    public class SqlQueryParameterContext : DbContext, IContext
    {
        public SqlQueryParameterContext() : base("ExporterDB")
        {
        }

        public SqlQueryParameterContext(string connectionString) : base(connectionString)
        {
        }

        public IDbSet<Parameter> Parameters { get; set; }
        public IDbSet<SqlQuery> SqlQueries { get; set; }
        public IDbSet<SqlQueryParameter> SqlQueryParameters { get; set; }
        public IDbSet<OutputTable> OutputTables { get; set; }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public void Save()
        {
            SaveChanges();
        }

        public void DbDispose()
        {
            Dispose();
        }

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            // base.OnModelCreating(modelBuilder);
            builder.Entity<Parameter>().HasKey(q => q.ParameterId);
            builder.Entity<SqlQuery>().HasKey(q => q.SqlQueryId);
            builder.Entity<OutputTable>().HasKey(t => t.OutputTableId);
            builder.Entity<SqlQueryParameter>().HasKey(q => q.SqlQueryParameterId);
            builder.Entity<SqlQueryParameter>()
                .Property(q => q.SqlQueryParameterId)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);

            // Relationships
            builder.Entity<SqlQueryParameter>()
                .HasRequired(t => t.Parameter)
                .WithMany(t => t.SqlQueriesParameters)
                .HasForeignKey<int>(t => t.ParameterId);

            builder.Entity<SqlQueryParameter>()
                .HasRequired(t => t.SqlQuery)
                .WithMany(t => t.SqlQueryParameters)
                .HasForeignKey<int>(t => t.SqlQueryId);
        }
    }
}