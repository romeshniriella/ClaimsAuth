using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using DinkLabs.ClaimsAuth.Web.Models;

namespace DinkLabs.ClaimsAuth.Web.Data
{
    public class ResourceRepository
    {
        private readonly ApplicationDbContext _db;

        public ResourceRepository()
        {
            _db = new ApplicationDbContext();
        }

        public int Save(List<ApplicationResource> resources)
        {
            EnsureApplicationExists();

            // get all existing/saved resources
            var existing = GetResources().ToList();
            // select only the ones which are new
            var itemsToInsert = resources.Where(item => !existing.Contains(item)).ToList();

            var result = 0;

            // go through each new resource and save them, one by one(for now.)
            foreach (var item in itemsToInsert)
            {
                try
                {
                    SaveResource(item);
                    result++;
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Unable to save resource: " + item.ToPermissionValue(), ex);
                }
            }

            // get all existing/saved global permissions
            var existingGlobal = GetResourceGlobalPermissions().ToList();
            // find the resources which can be accessed anonymously
            var anon = resources.Where(x => x.IsAnonymous).ToList();
            // get all the newly saved anonymous resources
            // because we have to add a record of them to global permissions table.
            var savedAnon = GetResources().Where(anon.Contains).ToList();

            foreach (var anonItem in savedAnon)
            {
                try
                {
                    // save only the new ones. new ones doesn't exist in the existing globals list. (yet)
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

        private void EnsureApplicationExists()
        {
            if (!_db.Application.Any(x=>x.ID == 1))
            {
                throw new ArgumentException("Please ensure you have one application in the Applications table with the ID = 1");
            }
        }

        public int SaveResourceGlobalPermission(ResourceGlobalPermission permission)
        {
            if (permission.ID > 0)
            {
                _db.Entry(permission).State = EntityState.Modified;
            }
            else
            {
                _db.ResourceGlobalPermission.Add(permission);
            }

            return _db.SaveChanges();
        }

        public int SaveResource(ApplicationResource resource)
        {
            if (resource.ID > 0)
            {
                _db.Entry(resource).State = EntityState.Modified;
            }
            else
            {
                _db.ApplicationResource.Add(resource);
            }

            return _db.SaveChanges();
        }

        public List<ApplicationResource> GetResources()
        {
            return _db.ApplicationResource.ToList();
        }

        public List<ResourceGlobalPermission> GetResourceGlobalPermissions()
        {
            return _db.ResourceGlobalPermission.ToList();
        }
    }
}