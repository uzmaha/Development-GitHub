namespace P_CMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssignedByField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Issues", "AssignedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Issues", "AssignedBy");
        }
    }
}
