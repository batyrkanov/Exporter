namespace Exporter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Parameters",
                c => new
                    {
                        ParameterId = c.Int(nullable: false, identity: true),
                        ParameterName = c.String(nullable: false),
                        ParameterType = c.String(nullable: false),
                        ParameterCreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ParameterId);
            
            CreateTable(
                "dbo.SqlQueryParameters",
                c => new
                    {
                        ParameterId = c.Int(nullable: false),
                        SqlQueryId = c.Int(nullable: false),
                        SqlQueryParameterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ParameterId, t.SqlQueryId })
                .ForeignKey("dbo.Parameters", t => t.ParameterId, cascadeDelete: true)
                .ForeignKey("dbo.SqlQueries", t => t.SqlQueryId, cascadeDelete: true)
                .Index(t => t.ParameterId)
                .Index(t => t.SqlQueryId);
            
            CreateTable(
                "dbo.SqlQueries",
                c => new
                    {
                        SqlQueryId = c.Int(nullable: false, identity: true),
                        SqlQueryName = c.String(nullable: false),
                        SqlQueryContent = c.String(nullable: false),
                        SqlQueryCreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SqlQueryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SqlQueryParameters", "SqlQueryId", "dbo.SqlQueries");
            DropForeignKey("dbo.SqlQueryParameters", "ParameterId", "dbo.Parameters");
            DropIndex("dbo.SqlQueryParameters", new[] { "SqlQueryId" });
            DropIndex("dbo.SqlQueryParameters", new[] { "ParameterId" });
            DropTable("dbo.SqlQueries");
            DropTable("dbo.SqlQueryParameters");
            DropTable("dbo.Parameters");
        }
    }
}
