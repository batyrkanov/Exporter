namespace Exporter.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Edits : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parameters", "ParameterRuName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.SqlQueries", "SqlQueryContent", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SqlQueries", "SqlQueryContent", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Parameters", "ParameterRuName");
        }
    }
}
