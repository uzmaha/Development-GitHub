namespace P_CMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewFieldcIssueTimeCounterIsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Issues", "timeCounter", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Issues", "timeCounter");
        }
    }
}
