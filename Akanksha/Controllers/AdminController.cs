using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Akanksha.Controllers
{
    public class AdminController : Controller
    {
        private AkankshaEntities db;

        public AdminController()
        {
            db = new AkankshaEntities();
        }

        // GET: Admin
       
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult AdminProfile()
        {
            
            var Id = Session["UserId"];
            Debug.WriteLine(Id);
            var user = db.AspNetUsers.Single(u => u.Id==Id);

            return View(user);
        }



        public ActionResult SaveProfile(AspNetUser user)
        {
            if (ModelState.IsValid)
            {
                var userInDb = db.AspNetUsers.Single(u => u.Id == user.Id);
                userInDb.Email = user.UserName;
                userInDb.UserName = user.UserName;
                userInDb.PhoneNumber = user.PhoneNumber;
                db.SaveChanges();
                return RedirectToAction("AdminProfile");

            }
            else
                {
                return View("EditAdminProfile", user);
            }


        }



        public ActionResult EditAdminProfile(string id)
        {
            var user = db.AspNetUsers.Single(u => u.Id == id);

            return View(user);
        }

      
        public ViewResult GetAllUsersProfile(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var users= from s in db.AspNetUsers
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.UserName.Contains(searchString)
                                       || s.Email.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderByDescending(s => s.UserName);
                    break;
                case "Date":
                    users = users.OrderBy(s => s.Email);
                    break;
                case "date_desc":
                    users = users.OrderByDescending(s => s.Email);
                    break;
                default:  // Name ascending 
                    users = users.OrderBy(s => s.UserName);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }


        [HttpPost]
        public JsonResult GetUsers(string keyword)
        {
            var users = db.AspNetUsers.Where(u => u.Email.ToLower().StartsWith(keyword.ToLower())).Select(a => a.Email).ToList();
            return Json(users, JsonRequestBehavior.AllowGet);
        }
    }
}