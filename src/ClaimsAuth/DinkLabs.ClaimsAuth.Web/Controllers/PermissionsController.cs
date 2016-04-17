using System.Web.Mvc;

namespace DinkLabs.ClaimsAuth.Web.Controllers
{
    public class PermissionsController : Controller
    {
        public ActionResult Global()
        {
            return View();
        }

        public ActionResult Group(string id)
        {
            return View();
        }
    }
}