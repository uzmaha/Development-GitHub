namespace P_CMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class relationinProductAndUserTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "ProductId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ProductId");
        }
    }
}
