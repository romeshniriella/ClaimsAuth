namespace DinkLabs.ClaimsAuth.Web.Models
{
    public class ResourceGlobalPermission
    {
        public ResourceGlobalPermission()
        {
        }

        public ResourceGlobalPermission(int resId)
        {
            ResourceID = resId;
            AllowAll = true;
        }

        public int ID { get; set; } // ID (Primary key)
        public int ResourceID { get; set; } // ResourceID
        public bool AllowAll { get; set; } // AllowAll
        // Reverse navigation


        // Foreign keys
        public virtual ApplicationResource ApplicationResource { get; set; }
    }
}