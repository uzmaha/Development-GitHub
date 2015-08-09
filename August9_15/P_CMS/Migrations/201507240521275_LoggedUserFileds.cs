namespace P_CMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoggedUserFileds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLoggeds", "LoggedAction", c => c.Byte(nullable: false));
            AlterColumn("dbo.Issues", "CreatedOn", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Issues", "CreatedOn", c => c.DateTime());
            DropColumn("dbo.UserLoggeds", "LoggedAction");
        }
    }
}
