using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DinkLabs.ClaimsAuth.Web.Models;

namespace DinkLabs.ClaimsAuth.Web.Controllers
{
    public class SecurityController : Controller
    {
        // GET: Security
        public ActionResult Index()
        {
            var db = new ApplicationDbContext();
            var users = db.Users.ToList();
            return View(users);
        }

        public ActionResult Permissions()
        {
            return View();
        }

        public ActionResult Groups()
        {
            return View();
        }

        public ActionResult Associations()
        {
            return View();
        }
    }
}