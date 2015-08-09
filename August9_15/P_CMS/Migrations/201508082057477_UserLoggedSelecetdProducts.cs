namespace P_CMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserLoggedSelecetdProducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserLoggeds", "selectedProducts", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserLoggeds", "selectedProducts");
        }
    }
}
