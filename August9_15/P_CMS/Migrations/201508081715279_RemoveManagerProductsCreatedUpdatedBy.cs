namespace P_CMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveManagerProductsCreatedUpdatedBy : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ManagerProducts", "CreatedBy");
            DropColumn("dbo.ManagerProducts", "UpdatedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ManagerProducts", "UpdatedBy", c => c.String());
            AddColumn("dbo.ManagerProducts", "CreatedBy", c => c.String());
        }
    }
}
