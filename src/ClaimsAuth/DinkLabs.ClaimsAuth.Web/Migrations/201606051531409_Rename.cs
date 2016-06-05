namespace DinkLabs.ClaimsAuth.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rename : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResourceGlobalPermissions", "ApplicationResource_ID", "dbo.ApplicationResources");
            DropIndex("dbo.ResourceGlobalPermissions", new[] { "ApplicationResource_ID" });
            DropColumn("dbo.ResourceGlobalPermissions", "ResourceID");
            RenameColumn(table: "dbo.ResourceGlobalPermissions", name: "ApplicationResource_ID", newName: "ResourceID");
            AlterColumn("dbo.ResourceGlobalPermissions", "ResourceID", c => c.Int(nullable: false));
            CreateIndex("dbo.ResourceGlobalPermissions", "ResourceID");
            AddForeignKey("dbo.ResourceGlobalPermissions", "ResourceID", "dbo.ApplicationResources", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ResourceGlobalPermissions", "ResourceID", "dbo.ApplicationResources");
            DropIndex("dbo.ResourceGlobalPermissions", new[] { "ResourceID" });
            AlterColumn("dbo.ResourceGlobalPermissions", "ResourceID", c => c.Int());
            RenameColumn(table: "dbo.ResourceGlobalPermissions", name: "ResourceID", newName: "ApplicationResource_ID");
            AddColumn("dbo.ResourceGlobalPermissions", "ResourceID", c => c.Int(nullable: false));
            CreateIndex("dbo.ResourceGlobalPermissions", "ApplicationResource_ID");
            AddForeignKey("dbo.ResourceGlobalPermissions", "ApplicationResource_ID", "dbo.ApplicationResources", "ID");
        }
    }
}
