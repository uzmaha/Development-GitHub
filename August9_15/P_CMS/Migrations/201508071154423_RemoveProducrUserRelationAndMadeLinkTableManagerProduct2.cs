namespace P_CMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProducrUserRelationAndMadeLinkTableManagerProduct2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ManagerProducts",
                c => new
                    {
                        ManagerProductId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ApplicationUserId = c.String(maxLength: 128),
                        CreatedBy = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        UpdatedBy = c.String(),
                        UpdatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ManagerProductId)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ManagerProducts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ManagerProducts", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.ManagerProducts", new[] { "ApplicationUserId" });
            DropIndex("dbo.ManagerProducts", new[] { "ProductId" });
            DropTable("dbo.ManagerProducts");
        }
    }
}
