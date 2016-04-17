using System.Collections.Generic;
using Newtonsoft.Json;

namespace DinkLabs.ClaimsAuth.Web.Models
{
    public class Application
    { 
        private List<ApplicationResource> _applicationResources;

        public int ID { get; set; } // ID (Primary key) 
        public string Description { get; set; } // Description 

        [JsonIgnore]
        public virtual List<ApplicationResource> ApplicationResources
        {
            get { return _applicationResources ?? (_applicationResources = new List<ApplicationResource>()); }
            set { _applicationResources = value; }
        }
    }
}