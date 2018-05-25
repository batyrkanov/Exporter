namespace Exporter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Deletedtypes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Types", "TypeId", "dbo.Parameters");
            DropIndex("dbo.Types", new[] { "TypeId" });
            DropTable("dbo.Types");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Types",
                c => new
                    {
                        TypeId = c.Int(nullable: false),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.TypeId);
            
            CreateIndex("dbo.Types", "TypeId");
            AddForeignKey("dbo.Types", "TypeId", "dbo.Parameters", "ParameterId");
        }
    }
}
