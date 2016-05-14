using System.Collections.Generic;
using System.Diagnostics;
using DinkLabs.ClaimsAuth.Web.Data;
using DinkLabs.ClaimsAuth.Web.Helpers;
using DinkLabs.ClaimsAuth.Web.Models;

namespace DinkLabs.ClaimsAuth.Web.Security
{
    /// <summary>
    /// Application Permission Manager. 
    /// Calling APM for shorter name
    /// </summary>
    public class APM
    {
        private static List<ApplicationResource> _resources;
        private static readonly object Sync = new object(); 
        private static volatile bool _initialized;
        public static List<ApplicationResource> Resources
        {
            get
            {
                if (_resources == null)
                {
                    Initialize();
                }
                return _resources;
            }
        }

        public static void SaveResources()
        {
            TaskHelper.RunBg(() =>
            {
                var resources = ResourceHelper.GetResources();
                Trace.WriteLine($"Saving {resources.Count} Resources");
                var repo = new ResourceRepository();
                repo.Save(resources);

                Initialize();
            });
        }

        private static void Initialize()
        { 
            if (!_initialized)
            {
                lock (Sync)
                {
                    if (!_initialized)
                    {
                        var repo = new ResourceRepository();
                        _resources = repo.GetResources();

                        _initialized = true;
                    }
                }
            }
        }


        public static void Refresh()
        {
            _initialized = false;
            Initialize();
        }

    }
}