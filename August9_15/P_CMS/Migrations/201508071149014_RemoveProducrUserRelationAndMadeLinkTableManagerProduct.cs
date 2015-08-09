namespace P_CMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProducrUserRelationAndMadeLinkTableManagerProduct : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Products", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Products", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.AspNetUsers", "ProductId");
            DropColumn("dbo.Products", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "ProductId", c => c.Int(nullable: false));
            CreateIndex("dbo.Products", "ApplicationUser_Id");
            AddForeignKey("dbo.Products", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
