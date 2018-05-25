namespace Exporter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExporterDB : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Parameters", "ParameterName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Parameters", "ParameterType", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.SqlQueries", "SqlQueryName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.SqlQueries", "SqlQueryContent", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SqlQueries", "SqlQueryContent", c => c.String(nullable: false));
            AlterColumn("dbo.SqlQueries", "SqlQueryName", c => c.String(nullable: false));
            AlterColumn("dbo.Parameters", "ParameterType", c => c.String(nullable: false));
            AlterColumn("dbo.Parameters", "ParameterName", c => c.String(nullable: false));
        }
    }
}
