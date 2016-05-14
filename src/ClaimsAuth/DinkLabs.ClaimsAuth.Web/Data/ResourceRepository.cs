using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DinkLabs.ClaimsAuth.Web.Models;

namespace DinkLabs.ClaimsAuth.Web.Data
{
    public class ResourceRepository
    {
        public int SaveResources(List<ApplicationResource> resources)
        {
            var existing = GetResources().ToList();
            var itemsToInsert = resources.Where(item => !existing.Contains(item)).ToList();
            var result = 0;
            foreach (var item in itemsToInsert)
            {
                try
                {
                    SaveResource(item);
                    result++;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unable to save resource: " + item, ex);
                }
            }

            var existingGlobal = GetResourceGlobalPermissions().ToList();
            var anon = resources.Where(x => x.IsAnonymous).ToList();
            var savedAnon = GetResources().Where(anon.Contains).ToList();

            foreach (var anonItem in savedAnon)
            {
                try
                {
                    if (existingGlobal.All(g => g.ResourceID != anonItem.ID))
                    {
                        SaveResourceGlobalPermission(new ResourceGlobalPermission(anonItem.ID));
                        result++;
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unable to save resource global permission: " + anonItem, ex);
                }
            }

            return result;
        }

        public List<ApplicationResource> GetResources()
        {
            var db = new ApplicationDbContext();
            return db.ApplicationResource.ToList();
        }
    }
}