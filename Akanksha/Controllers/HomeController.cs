using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Akanksha.Controllers
{
    public class HomeController : Controller
    {
        private AkankshaEntities db;
        public HomeController()
        {
            db = new AkankshaEntities();
        }
        public ActionResult Index()
        {
            var topcategories = db.Subcategories.Where(s => s.ParentId == null).Take(22);
            return View(topcategories);
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}