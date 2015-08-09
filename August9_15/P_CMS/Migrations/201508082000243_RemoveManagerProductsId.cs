namespace P_CMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveManagerProductsId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ManagerProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.ManagerProducts", new[] { "ProductId" });
            AddColumn("dbo.ManagerProducts", "ProductIds", c => c.String());
            DropColumn("dbo.ManagerProducts", "ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ManagerProducts", "ProductId", c => c.Int(nullable: false));
            DropColumn("dbo.ManagerProducts", "ProductIds");
            CreateIndex("dbo.ManagerProducts", "ProductId");
            AddForeignKey("dbo.ManagerProducts", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
    }
}
