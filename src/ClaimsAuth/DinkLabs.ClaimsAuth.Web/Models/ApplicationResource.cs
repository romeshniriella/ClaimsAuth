using System.Collections.Generic;

namespace DinkLabs.ClaimsAuth.Web.Models
{
    public class ApplicationResource
    {
        // Reverse navigation

        private List<ResourceGlobalPermission> _resourceGlobalPermissions;
        private List<ResourceRolePermission> _resourceRolePermissions;

        //public ApplicationResource()
        //{
        //}

        //public ApplicationResource(int applicationId, string area, string controller, string action, bool post, bool api)
        //{
        //    if (applicationId <= 0)
        //    {
        //        throw new ArgumentException("Please provide a valid application ID in configuration");
        //    }

        //    ApplicationID = applicationId;
        //    Area = area;
        //    Controller = controller;
        //    Action = action;
        //    IsGetAction = !post;
        //    IsApiAction = api;

        //    Description = "/" + Area.IfNotBlank(a => a + "/") + Controller.IfNotBlank(a => a + "/") +
        //                  Action.IfNotBlank(a => a);
        //}

        public int ID { get; set; } // ID (Primary key)
        public int ApplicationID { get; set; } // ApplicationID
        public string Area { get; set; } // Area
        public string Controller { get; set; } // Controller
        public string Action { get; set; } // Action
        public string Description { get; set; } // Description
        public bool? IsGetAction { get; set; } // IsGetAction
        public bool? IsApiAction { get; set; } // IsApiAction
        public bool IsAnonymous { get; set; }
        public string Role { get; set; }

        public virtual List<ResourceGlobalPermission> ResourceGlobalPermissions
        {
            get
            {
                return _resourceGlobalPermissions ?? (_resourceGlobalPermissions = new List<ResourceGlobalPermission>());
            }
            set { _resourceGlobalPermissions = value; }
        }

        public virtual List<ResourceRolePermission> ResourceRolePermissions
        {
            get { return _resourceRolePermissions ?? (_resourceRolePermissions = new List<ResourceRolePermission>()); }
            set { _resourceRolePermissions = value; }
        }

        // Foreign keys
        public virtual  Application Application { get; set; } //  FK_ApplicationResource_Application

        //public override int GetHashCode()
        //{
        //    return Area.IfNotNull(a => a.GetHashCode())
        //           ^ Controller.IfNotNull(a => a.GetHashCode())
        //           ^ Action.IfNotNull(a => a.GetHashCode())
        //           ^ IsGetAction.GetValueOrDefault().GetHashCode()
        //           ^ IsApiAction.GetValueOrDefault().GetHashCode();
        //}

        //public override bool Equals(object obj)
        //{
        //    return GetHashCode() == obj.GetHashCode();
        //}

        //public string ToPermissionValue()
        //{
        //    var req = "/" + Area.IfNotBlank(a => a + "/") + Controller.IfNotBlank(a => a + "/") +
        //              Action.IfNotBlank(a => a);

        //    return "{0}|{1}|{2}".FormatWith(IsGetAction.GetValueOrDefault() ? "GET" : ""
        //        , req, IsApiAction.GetValueOrDefault() ? "API" : "");
        //}
    }
}