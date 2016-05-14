using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using DinkLabs.ClaimsAuth.Web.Models;
using Microsoft.Ajax.Utilities;
using AllowAnonymousAttribute = System.Web.Http.AllowAnonymousAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using NonActionAttribute = System.Web.Http.NonActionAttribute;

namespace DinkLabs.ClaimsAuth.Web.Security
{

    /// <summary>
    ///     Extract all MVC/WebApi area/controller/actions and related properties
    /// </summary>
    public class ResourceHelper
    {
        private const string NamespacePrefix = "oklo";
        private static int _appId; 
        public static int ApplicationID
        {
            get
            {
                // retrieve the application ID form app-settings
                return _appId > 0 ? _appId : (_appId = 1);
            }
        }

        /// <summary>
        ///     Retrive MVC/WebApi Area/Controller/Actions from the system
        /// </summary>
        /// <returns></returns>
        public static List<ApplicationResource> GetResources()
        {
            var resources = new List<ApplicationResource>();

            try
            {
                // add MVC resources
                resources.AddRange(MapResources<IController>());
                // Add web-api resources
                resources.AddRange(MapResources<IHttpController>(true));
            }
            catch (ReflectionTypeLoadException ex)
            {
                LogReflectionEx(ex);
            }

            return resources;
        }

        private static void LogReflectionEx(ReflectionTypeLoadException ex)
        {
            // something went wrong in scanning, can't find required types
            var sb = new StringBuilder();
            foreach (var exSub in ex.LoaderExceptions)
            {
                sb.AppendLine(exSub.Message);
                var exFileNotFound = exSub as FileNotFoundException;
                if (exFileNotFound != null)
                {
                    if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                    {
                        sb.AppendLine("Fusion Log:");
                        sb.AppendLine(exFileNotFound.FusionLog);
                    }
                }
                sb.AppendLine();
            }
            var errorMessage = sb.ToString();
            //Display or log the error based on your application.
            Trace.TraceError(errorMessage); 
        }

        /// <summary>
        ///     Scan and map resources of a given type deined by T
        /// </summary>
        /// <typeparam name="T">Type of resource to be scanned</typeparam>
        /// <param name="api">Is this a API scan request?</param>
        /// <returns>Set of application resources</returns>
        private static IEnumerable<ApplicationResource> MapResources<T>(bool api = false)
        {
            // Class naming Convention : MVC controllers and API Controllers.
            var cSuffix = api ? "ApiController" : "Controller";

            // currently loaded assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Retrieve all area registrations
            var areaRegistrations = GetAreaRegistrations(assemblies).ToList();

            // get controllers
            var controllerTypes = GetControllerTypes<T>(assemblies, cSuffix);

            // get all actions for above controller
            var controllerMethods = GetControllerActions(controllerTypes, api);

            var result = new List<ApplicationResource>();

            // go thru each controller
            foreach (var controllerType in controllerMethods)
            {
                try
                {
                    // get area name
                    var area = GetAreaName(controllerType, areaRegistrations);
                    // get controller name
                    var controller = controllerType.Key.Name.Replace("Controller", "");

                    // add area, add controller
                    var areaRes = new ApplicationResource(ApplicationID, area, null, null, true, false);
                    var ctrlRes = new ApplicationResource(ApplicationID, area, controller, null, true, api);

                    // we dont need to add duplicate area sections
                    if (!result.Contains(areaRes))
                    {
                        result.Add(areaRes);
                    }

                    // we dont wanna add duplicate controller sections either.
                    if (!result.Contains(ctrlRes))
                    {
                        // get the attributes
                        var ctrlAttrs = controllerType.Key.GetCustomAttributes(false);
                        // check if this allows anonymous actions.
                        // api | mvc seperated
                        var isAnon = ctrlAttrs.Any(a => a.GetType().IsAssignableFrom(typeof(AllowAnonymousAttribute))
                                                        ||
                                                        a.GetType()
                                                            .IsAssignableFrom(
                                                                typeof(System.Web.Mvc.AllowAnonymousAttribute)));
                        ctrlRes.IsAnonymous = isAnon;
                        result.Add(ctrlRes);
                    }

                    // go thru each action.
                    foreach (var action in controllerType.Value)
                    {
                        // get ction name
                        var actionName = action.Name;
                        // get action attributes
                        var actions = action.GetCustomAttributes(false);
                        // check if this is a get or post or smtn else.
                        var attrGet = actions.Any(a => a.GetType().IsAssignableFrom(typeof(HttpPostAttribute))
                                                       ||
                                                       a.GetType()
                                                           .IsAssignableFrom(typeof(System.Web.Mvc.HttpPostAttribute)));
                        // check if this is allowed anonymous access
                        var isAnon = actions.Any(a => a.GetType().IsAssignableFrom(typeof(AllowAnonymousAttribute))
                                                      ||
                                                      a.GetType()
                                                          .IsAssignableFrom(typeof(System.Web.Mvc.AllowAnonymousAttribute)));
                        // add the resource
                        result.Add(new ApplicationResource(ApplicationID, area, controller, actionName, attrGet, api)
                        {
                            IsAnonymous = isAnon
                        });
                    }
                }
                catch (ReflectionTypeLoadException ex)
                { 
                    LogReflectionEx(ex);
                }
            }

            return result;
        }

        /// <summary>
        ///     Retrieve area name from controllers
        /// </summary>
        /// <param name="controllerType"></param>
        /// <param name="areaRegistrations"></param>
        /// <returns></returns>
        private static string GetAreaName(
            KeyValuePair<Type, IEnumerable<MethodInfo>> controllerType,
            IEnumerable<Type> areaRegistrations)
        {
            // check whether the namespace has 'Areas', by convention
            if (controllerType.Key.Namespace != null && controllerType.Key.Namespace.Contains("Areas"))
            {
                // namespace
                var areaNamespace = controllerType.Key.Namespace;
                // from all area registrations, find the above namespace
                var regArea = areaRegistrations.First(a => a.Namespace != null && areaNamespace.Contains(a.Namespace));
                // from the are registration, get the area name property(defined in a property)
                var areaNameProp = regArea.GetProperty("AreaName");
                // we need to get the area name value so we have to create an object of the above area registration
                var instance = Activator.CreateInstance(regArea);
                // get the value from the property
                var areaNameValue = areaNameProp.GetValue(instance);
                return areaNameValue.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        ///     Retrieves all controller actions
        /// </summary>
        /// <param name="controllerTypes"></param>
        /// <param name="api"></param>
        /// <returns></returns>
        private static Dictionary<Type, IEnumerable<MethodInfo>> GetControllerActions(IEnumerable<Type> controllerTypes,
            bool api)
        {
            // create a dictionary where [Key = controller, Values = Actions]
            var controllerMethods = controllerTypes.ToDictionary(
                controllerType => controllerType, // dic key
                controllerType =>
                    controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        // get public and instance methods
                        .Where(m => // filter using the following criteria
                                    // if we want API then use Http Non-Action attribute otherwise us the MVC one
                            (api
                                ? !m.IsDefined(typeof(NonActionAttribute))
                                : !m.IsDefined(typeof(System.Web.Mvc.NonActionAttribute)))
                            // if we want API then we are using HttpResponseMessage in every API action. otherwise use ActionResult
                            &&
                            (api
                                ? typeof(HttpResponseMessage).IsAssignableFrom(m.ReturnType)
                                : (typeof(ActionResult).IsAssignableFrom(m.ReturnType) ||
                                   typeof(Task<ActionResult>).IsAssignableFrom(m.ReturnType)))
                            && m.Name != "Dispose" // don't get Dispose method
                            && !m.IsSpecialName // no special names (from get/set auro properties)
                            && !m.IsStatic)); // no static methods
            return controllerMethods;
        }

        /// <summary>
        ///     Get controller types for given assemblies set
        /// </summary>
        /// <typeparam name="T">Controller Type to scan</typeparam>
        /// <param name="assemblies"></param>
        /// <param name="controllerSuffix">Api or Mvc?</param>
        /// <returns></returns>
        private static IEnumerable<Type> GetControllerTypes<T>(IEnumerable<Assembly> assemblies,
            string controllerSuffix = "Controller")
        {
            var controllerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t != null
                            && t.IsPublic // public controllers only
                            && t.Name.EndsWith(controllerSuffix, StringComparison.OrdinalIgnoreCase)
                            // enfore naming convention
                            && t.Namespace.IfNotNull(n => n.ToLowerInvariant().StartsWith(NamespacePrefix))
                            // get only our OKLO web controllers
                            && !t.IsAbstract // no abstract controllers
                            && typeof(T).IsAssignableFrom(t)); // must implement the type we requested
                                                               // should implement T (happens automatically when you extend Controller/ApiController)
            return controllerTypes;
        }

        /// <summary>
        ///     Retrieve all area registrations
        /// </summary>
        /// <param name="assemblies">Assemblies to scan</param>
        /// <returns></returns>
        private static IEnumerable<Type> GetAreaRegistrations(IEnumerable<Assembly> assemblies)
        {
            // what this does is,
            // 1. get all the types in the given assemblies set
            // 2. filter out the types which implements AreaRegistration class.
            var areaRegistrations = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t != null && typeof(AreaRegistration).IsAssignableFrom(t));
            return areaRegistrations;
        }
    }
}