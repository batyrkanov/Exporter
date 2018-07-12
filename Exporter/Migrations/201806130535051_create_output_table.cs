namespace Exporter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create_output_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OutputTables",
                c => new
                    {
                        OutputTableId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        FileName = c.String(),
                        FileType = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                        SqlQueryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OutputTableId)
                .ForeignKey("dbo.SqlQueries", t => t.SqlQueryId, cascadeDelete: true)
                .Index(t => t.SqlQueryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OutputTables", "SqlQueryId", "dbo.SqlQueries");
            DropIndex("dbo.OutputTables", new[] { "SqlQueryId" });
            DropTable("dbo.OutputTables");
        }
    }
}
