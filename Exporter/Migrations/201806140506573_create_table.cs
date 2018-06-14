namespace Exporter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_table : DbMigration
    {
        public override void Up()
        {
            DropTable("SqlQueryParameters");

            CreateTable(
               "dbo.SqlQueryParameters",
               c => new
                    {
                        SqlQueryParameterId = c.Int(nullable: false, identity: true),
                        ParameterId = c.Int(nullable: false),
                        SqlQueryId = c.Int(nullable: false),
                    })
               .PrimaryKey(t => t.SqlQueryParameterId)
               .ForeignKey("dbo.SqlQueries", t => t.SqlQueryId, cascadeDelete: true)
               .ForeignKey("dbo.Parameters", t => t.ParameterId, cascadeDelete: true)
               .Index(t => t.ParameterId)
               .Index(t => t.SqlQueryId);
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.SqlQueryParameters");
            DropForeignKey("dbo.SqlQueryParameters", "SqlQueryId", "dbo.SqlQueries");
            DropForeignKey("dbo.SqlQueryParameters", "ParameterId", "dbo.Parameters");
            DropIndex("dbo.SqlQueryParameters", new[] { "SqlQueryId", "ParameterId" });
            DropTable("dbo.SqlQueryParameters");
        }
    }
}
