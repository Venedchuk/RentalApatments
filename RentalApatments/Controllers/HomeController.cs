using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using RentalApatments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RentalApatments.Controllers
{
    //var name = User.Identity.GetUserId();
    public class HomeController : Controller
    {
        private ApplicationContext db = new ApplicationContext();
        public ActionResult Index()
        {
            
            return View();
        }

        //[Authorize]
        //public ActionResult Index()
        //{
        //    IList<string> roles = new List<string> { "Роль не определена" };
        //    ApplicationUserManager userManager = HttpContext.GetOwinContext()
        //                                            .GetUserManager<ApplicationUserManager>();
        //    ApplicationUser user = userManager.FindByEmail(User.Identity.Name);
        //    if (user != null)
        //        roles = userManager.GetRoles(user.Id);
        //    return View(roles);
        //}

        //[Authorize(Roles = "admin")]
        public ActionResult About()
        {
            ViewBag.Message = "Description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact page.";

            return View();
        }
    }
}