namespace P_CMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssignedOnField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Issues", "AssignedOn", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Issues", "AssignedOn");
        }
    }
}
