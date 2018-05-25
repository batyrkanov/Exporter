namespace Exporter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Type : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Types",
                c => new
                    {
                        TypeId = c.Int(nullable: false),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.TypeId)
                .ForeignKey("dbo.Parameters", t => t.TypeId)
                .Index(t => t.TypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Types", "TypeId", "dbo.Parameters");
            DropIndex("dbo.Types", new[] { "TypeId" });
            DropTable("dbo.Types");
        }
    }
}
