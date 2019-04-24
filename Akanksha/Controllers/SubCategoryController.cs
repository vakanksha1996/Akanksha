using Akanksha.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Akanksha.Controllers
{
    public class SubCategoryController : Controller
    {
        private AkankshaEntities db;

        public SubCategoryController()
        {
            db = new AkankshaEntities();
        }


        // GET: SubCategory
        [HttpGet]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
          //  ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var subcategories = from s in db.Subcategories
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                subcategories = subcategories.Where(s => s.Name.ToLower().StartsWith(searchString.ToLower()));


            }
            switch (sortOrder)
            {
                case "name_desc":
                    subcategories = subcategories.OrderByDescending(s => s.Name);
                    break;
               
                default:  // Name ascending 
                    subcategories = subcategories.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(subcategories.ToPagedList(pageNumber, pageSize));
        }



        public JsonResult Getsubcategories(string keyword)
        {
            List<string> subcategories;
            subcategories = db.Subcategories.Where(x => x.Name.ToLower().StartsWith(keyword.ToLower())).Select(y => y.Name).ToList();

            return Json(subcategories, JsonRequestBehavior.AllowGet); 
        }




        public ActionResult New()

        {
            var vm = new SubcategoryViewModel
            {
                DepartmentList = db.Categories.ToList(),
                CategoryList = db.Subcategories.ToList()
                             

            };
            return View(vm);
        }



        [HttpPost]
        public ActionResult Save(Subcategory subcategory)
        {
           
            if (subcategory.SubcategoryId == 0)
            {
                if (ModelState.IsValid)
                {
                    subcategory.CreatedDate = DateTime.Now;
                    db.Subcategories.Add(subcategory);
                    db.SaveChanges();

                }
                else
                {
                    var vm = new SubcategoryViewModel
                    {
                        SubCategory=subcategory,
                        DepartmentList = db.Categories.ToList(),
                        CategoryList = db.Subcategories.ToList()


                    };
                    return View("New",vm);
                }
               


            }
            else
            {
                if (ModelState.IsValid)
                {
                    var dbsubcategory = db.Subcategories.Single(s => s.SubcategoryId == subcategory.SubcategoryId);
                    dbsubcategory.Name = subcategory.Name;
                    dbsubcategory.ModifiedDate = DateTime.Now;
                    dbsubcategory.Pic = subcategory.Pic;
                    dbsubcategory.ParentId = subcategory.ParentId;
                   dbsubcategory.Description = subcategory.Description;
                    dbsubcategory.CategoryId = subcategory.CategoryId;
                   
                    db.SaveChanges();
                }
                else
                {
                    var vm = new SubcategoryViewModel
                    {
                        SubCategory = subcategory,
                        DepartmentList = db.Categories.ToList(),
                        CategoryList = db.Subcategories.ToList()


                    };
                    return View("Edit", vm);
                }
               

            }
            return RedirectToAction("Index", "SubCategory");
            
        }



        public ActionResult Edit(int id)
        {
            TempData.Remove("ProductAlert");
            TempData.Remove("SubcategoryAlert");

            var subcategory = db.Subcategories.Single(s => s.SubcategoryId == id);
            var vm = new SubcategoryViewModel
            {
                SubCategory=subcategory,
                DepartmentList = db.Categories.ToList(),
                CategoryList = db.Subcategories.
                             ToList()

            };
            return View(vm);
        }



        public JsonResult Delete(int Id)
        {
            var category = db.Subcategories.Single(c => c.SubcategoryId == Id);
            var productcount = db.Products.Count(p => p.Subcategory.Name == category.Name);
            var subcategoriescount = db.Subcategories.Count(s => s.ParentId == category.SubcategoryId);
            if (category == null)
            {

                throw new Exception();
            }
            if (productcount != 0) {



                return Json("ProductAlert", JsonRequestBehavior.AllowGet);
            }
            if(subcategoriescount != 0)
            {

                return Json("SubcategoryAlert", JsonRequestBehavior.AllowGet);
            }
            db.Subcategories.Remove(category);
            db.SaveChanges();
            var categories = db.Subcategories.ToList();
            return Json("success", JsonRequestBehavior.AllowGet);
        }


        public ActionResult  GetSubcategoriesByCategoryName(string CategoryName,int? ParentId)

        {
            IEnumerable<Subcategory> subcategories;
            if (ParentId == null)
            {
                 subcategories = db.Subcategories.Where(s => s.Category.Name == CategoryName && s.ParentId==null).ToList();

            }
            else
            {
                subcategories = db.Subcategories.Where(s => s.ParentId == ParentId).ToList();
                   
                
            }
            return View(subcategories);
        }
     
    }
}