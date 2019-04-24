using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Akanksha.Controllers
{
    public class CategoryController : Controller
    {
        private AkankshaEntities db;
        public CategoryController()
        {
            db = new AkankshaEntities();
        }


        // GET: Category
        [HttpGet]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var categories = from s in db.Categories
                                select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                categories = categories.Where(s => s.Name.ToLower().StartsWith(searchString.ToLower()));

            }
            switch (sortOrder)
            {
                case "name_desc":
                    categories = categories.OrderByDescending(s => s.Name);
                    break;

                default:  // Name ascending 
                    categories = categories.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 4;
            int pageNumber = (page ?? 1);
            return View(categories.ToPagedList(pageNumber, pageSize));
        }



        public JsonResult GetCategories(string keyword)
        {
            List<string> categories;
            categories = db.Categories.Where(x => x.Name.ToLower().StartsWith(keyword.ToLower())).Select(y => y.Name).ToList();

            return Json(categories, JsonRequestBehavior.AllowGet);
        }



        public ActionResult New()
        {
            
            return View();
        }



        [HttpGet]
        public ActionResult GetCategories()
        {
            
            List<Category> categories = new List<Category>();
            categories = db.Categories.ToList();

            return View(categories);
        }



        public ActionResult Save(Category category)
        {
           
            
                if (category.CategoryId == 0)
                {
                    if (ModelState.IsValid)
                    {
                        category.CreatedDate = DateTime.Now;
                        db.Categories.Add(category);
                        db.SaveChanges();

                    }
                    else
                    {
                        return View("New");
                    }


                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        var dbcategory = db.Categories.Single(c => c.CategoryId == category.CategoryId);
                        dbcategory.Name = category.Name;
                        dbcategory.Pic = category.Pic;
                        dbcategory.Description = category.Description;
                        db.SaveChanges();

                    }
                    else
                    {
                        return View("Edit", category);
                    }

                }
            return RedirectToAction("Index");

      }
           

        

        public ActionResult Edit(int Id)
        {
            var category = db.Categories.Single(c => c.CategoryId == Id);
            return View(category);
        }



        public JsonResult Delete(int Id)
        {
            var department = db.Categories.Single(c => c.CategoryId == Id);
            var categorycount = db.Subcategories.Count(s => s.CategoryId == department.CategoryId);
            if (department == null)
            {

                throw new Exception();
            }

            if(categorycount != 0)
            {
                return Json("alert", JsonRequestBehavior.AllowGet);
            }
            db.Categories.Remove(department);
            db.SaveChanges();
            return Json("success", JsonRequestBehavior.AllowGet);
        }
    }
}