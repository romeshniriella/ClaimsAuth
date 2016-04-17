using System;
using System.Data.Entity;
using System.Net.Mime;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DinkLabs.ClaimsAuth.Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public IDbSet<Application> Application { get; set; }
        public IDbSet<ApplicationResource> ApplicationResource { get; set; }
        public IDbSet<ResourceGlobalPermission> ResourceGlobalPermission { get; set; }
        public IDbSet<ResourceRolePermission> ResourceRolePermission { get; set; }
    
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}