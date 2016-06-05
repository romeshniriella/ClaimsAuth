namespace DinkLabs.ClaimsAuth.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ApplicationResources",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ApplicationID = c.Int(nullable: false),
                        Area = c.String(),
                        Controller = c.String(),
                        Action = c.String(),
                        Description = c.String(),
                        IsGetAction = c.Boolean(),
                        IsApiAction = c.Boolean(),
                        IsAnonymous = c.Boolean(nullable: false),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Applications", t => t.ApplicationID, cascadeDelete: true)
                .Index(t => t.ApplicationID);
            
            CreateTable(
                "dbo.ResourceGlobalPermissions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ResourceID = c.Int(nullable: false),
                        AllowAll = c.Boolean(nullable: false),
                        ApplicationResource_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ApplicationResources", t => t.ApplicationResource_ID)
                .Index(t => t.ApplicationResource_ID);
            
            CreateTable(
                "dbo.ResourceRolePermissions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ResourceID = c.Int(nullable: false),
                        RoleID = c.String(),
                        Allow = c.Boolean(nullable: false),
                        ApplicationResource_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ApplicationResources", t => t.ApplicationResource_ID)
                .Index(t => t.ApplicationResource_ID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ResourceRolePermissions", "ApplicationResource_ID", "dbo.ApplicationResources");
            DropForeignKey("dbo.ResourceGlobalPermissions", "ApplicationResource_ID", "dbo.ApplicationResources");
            DropForeignKey("dbo.ApplicationResources", "ApplicationID", "dbo.Applications");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ResourceRolePermissions", new[] { "ApplicationResource_ID" });
            DropIndex("dbo.ResourceGlobalPermissions", new[] { "ApplicationResource_ID" });
            DropIndex("dbo.ApplicationResources", new[] { "ApplicationID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ResourceRolePermissions");
            DropTable("dbo.ResourceGlobalPermissions");
            DropTable("dbo.ApplicationResources");
            DropTable("dbo.Applications");
        }
    }
}
