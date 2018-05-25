using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Exporter.Models.Entities
{
    public class SqlQueryParameterContext : DbContext
    {
        public SqlQueryParameterContext() : base("ExporterDB")
        {
        }

        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<SqlQuery> SqlQueries { get; set; }
        public DbSet<SqlQueryParameter> SqlQueriesParameters { get; set; }
        protected override void OnModelCreating(DbModelBuilder builder)
        {
            // base.OnModelCreating(modelBuilder);
            builder.Entity<Parameter>().HasKey(q => q.ParameterId);
            builder.Entity<SqlQuery>().HasKey(q => q.SqlQueryId);
            builder.Entity<SqlQueryParameter>().HasKey(q =>
                new {
                    q.ParameterId,
                    q.SqlQueryId
                });

            // Relationships
            builder.Entity<SqlQueryParameter>()
                .HasRequired(t => t.Parameter)
                .WithMany(t => t.SqlQueriesParameters)
                .HasForeignKey(t => t.ParameterId);

            builder.Entity<SqlQueryParameter>()
                .HasRequired(t => t.SqlQuery)
                .WithMany(t => t.SqlQueriesParameters)
                .HasForeignKey(t => t.SqlQueryId);
            
        }
    }
}