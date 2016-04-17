namespace DinkLabs.ClaimsAuth.Web.Models
{
    public class ResourceRolePermission
    {
        public int ID { get; set; } // ID (Primary key)
        public int ResourceID { get; set; } // ResourceID
        public string RoleID { get; set; } // RoleID
        public bool Allow { get; set; } // Allow
        // Reverse navigation


        // Foreign keys
        public virtual ApplicationResource ApplicationResource { get; set; } 
    }
}