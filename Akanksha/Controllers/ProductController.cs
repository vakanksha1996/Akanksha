using Akanksha.Models;
using Akanksha.ViewModel;
using PagedList;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Net;
using System.Net.Mail;

namespace Akanksha.Controllers
{
    public class ProductController : Controller
    {
        private AkankshaEntities db;
        
        public ProductController()
        {
            db = new Akanksha.AkankshaEntities();
        }



        // GET: Product
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "Price_desc" : "Price";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            int pageSize = 6;
            int pageNumber = (page ?? 1);
            int startlimit = (pageNumber - 1) * pageSize+1;
            int endlimit = (pageNumber * pageSize);
            int pageCount = 2;
            int Srno = 0;
            var columns = db.Products.AsEnumerable().Select((p, RowNumber) => new
            {
                RowNumber = RowNumber + 1,
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Pic = p.Pic,
                Price = p.Price,
                DiscountRate = p.DiscountRate,
                SubcategoryId = p.SubcategoryId,
                NumberOfStock = p.NumberOfStock,
                CreatedDate = p.CreatedDate,
                ModifiedDate = p.ModifiedDate
            });


            int totalcount = columns.Count();



            //IEnumerable<Product> products = (from p in columns
            //                           where p.RowNumber >= startlimit && p.RowNumber <= endlimit
            //                           select new Product
            //                           {
            //                               ProductId = p.ProductId,
            //                               Name = p.Name,
            //                               Description = p.Description,
            //                               Pic = p.Pic,
            //                               Price = p.Price,
            //                               DiscountRate = p.DiscountRate,
            //                               SubcategoryId = p.SubcategoryId,
            //                               NumberOfStock = p.NumberOfStock
            //                           });

            IEnumerable<Product> products = db.Products.ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.ToLower().StartsWith(searchString.ToLower()));

            }
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "Price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:  // Name ascending 
                    products = products.OrderBy(s => s.Name);
                    break;
            }
            ViewData["pageSize"] = pageSize;
            ViewData["PageNumber"] = pageNumber;
            ViewData["TC"] = totalcount;

           

            return View(products.ToPagedList(pageNumber, pageSize));
        }



        [HttpPost]
        public JsonResult GetProducts(string keyword)
        {

            List<string> products;
             products = db.Products.Where(x => x.Name.ToLower().StartsWith(keyword.ToLower())).Select(y => y.Name).ToList();
            return  Json(products,JsonRequestBehavior.AllowGet);
             
        }



        public ActionResult New()
        {
            var parentids = db.Subcategories.Where(p => p.ParentId != null).Select(p=>p.ParentId).ToList();
           
            var vm = new ProductViewModel
            {

                SubcategoryList = db.Subcategories
                             .Where(e => !parentids.Contains(e.SubcategoryId)).ToList()


            };
            return View(vm);
                                                                                                                                                                                                                                                                                                  

        }



        public ActionResult Edit(int Id)
        {
            var product = db.Products.Single(p => p.ProductId == Id);
            var parentids = db.Subcategories.Where(p => p.ParentId != null).Select(p => p.ParentId).ToList();

            if (product==null)
            {
                throw new Exception();
            }
            var pvm = new ProductViewModel
            {
                Product = product,
                SubcategoryList = db.Subcategories
                             .Where(e => !parentids.Contains(e.SubcategoryId)).ToList()
            };
            return View(pvm);
        }



        public ActionResult Save(Product product)
        {
            var parentids = db.Subcategories.Where(p => p.ParentId != null).Select(p => p.ParentId).ToList();

            if (product.ProductId == 0)
            {
                if (ModelState.IsValid)
                {
                    product.CreatedDate = DateTime.Now;
                    db.Products.Add(product);
                    db.SaveChanges();

                }

                else
                {
                    var vm = new ProductViewModel
                    {
                        Product=product,
                        SubcategoryList = db.Subcategories
                             .Where(e => !parentids.Contains(e.SubcategoryId)).ToList()


                    };
                    return View("New",vm);
                }
              
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var dbproduct = db.Products.Single(p => p.ProductId == product.ProductId);
                    dbproduct.Name = product.Name;
                    dbproduct.ModifiedDate = DateTime.Now;
                    dbproduct.Pic = product.Pic;
                    dbproduct.Description = product.Description;
                    dbproduct.Price = product.Price;
                    dbproduct.NumberOfStock = product.NumberOfStock;
                    dbproduct.SubcategoryId = product.SubcategoryId;
                    db.SaveChanges();

                }
                else
                {
                    var vm = new ProductViewModel
                    {
                        Product = product,
                        SubcategoryList = db.Subcategories
                            .Where(e => !parentids.Contains(e.SubcategoryId)).ToList()


                    };

                    return View("Edit", vm);
                }
               
            }
            return RedirectToAction("Index");
        }



        public JsonResult Delete(int Id)
        {
            var product = db.Products.Single(p => p.ProductId == Id);
            if (product == null)
            {
                throw new Exception();
            }
            db.Products.Remove(product);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }



        public ActionResult GetProductsByName(string name,string sortOrder, string currentFilter, string searchString, int? page) 
        {
                  
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameDescParm = "name_desc";
            ViewBag.NameAsenParm = "name_asen";
            ViewBag.PriceAsenParm = "Price";
            ViewBag.PriceDescParm = "price_desc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            IEnumerable<Product> products;
            var parentcategory = db.Subcategories.Single(s => s.Name == name);


                products = (from s in db.Products.Where(p => p.Subcategory.Name == name)
                            select s);
                if (products.Count() == 0)
                {
                   return RedirectToAction("GetSubcategoriesByCategoryName", "SubCategory", new {CategoryName=name, ParentId =  parentcategory.SubcategoryId});
                }
            
        
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));

            }
            switch (sortOrder)
            {
                case "name_asen":

                    products = products.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:  // Name ascending 
                    products = products.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));

           
        }



        public ActionResult ProductPage(int id)
        {
            string Id = User.Identity.GetUserId();
            ViewBag.IsItemaddedtoCart = db.Carts.Count(p => p.ProductId == id && p.Id==Id);
            Session["isitemadded"] = db.Carts.Count(p => p.ProductId == id && p.Id == Id);
            var product = db.Products.Single(p => p.ProductId == id);
            return View(product);
        }


        [Authorize]
        public ActionResult CheckOut(Address address,int productid)
        {
            string id = User.Identity.GetUserId();
            
            var cvm = new BuyViewModel
            {
                Product = db.Products.Single(p=>p.ProductId==productid),
                States = db.States.ToList(),
                PaymentModes = db.Payments.ToList(),
                AddressBook = db.Addresses.Where(a => a.Id == id).ToList()
            };

            if (address.AddressId != 0)
            {
                if (address.City == null)
                {
                    cvm.address = db.Addresses.Single(a => a.AddressId == address.AddressId);

                }
                else
                {
                    var addressindb = db.Addresses.Single(a => a.AddressId == address.AddressId);
                    addressindb.HouseNo = address.HouseNo;
                    addressindb.City = address.City;
                    addressindb.Colony_Street = address.Colony_Street;
                    addressindb.StateId = address.StateId;
                    addressindb.Pincode = address.Pincode;
                    db.SaveChanges();
                    cvm.address = addressindb;
                }
            }

            if (address.City != null && address.AddressId == 0)
            {
                var user = db.AspNetUsers.Single(u => u.Id == id);
                address.Id  =user.Id;
                db.Addresses.Add(address);
                db.SaveChanges();
                cvm.address = address;
            }
            return View(cvm);
        }



        public ActionResult OrderCompletion(int addressId, Order Order,int productId)
        {
            string id = User.Identity.GetUserId();
            var address = db.Addresses.Single(a => a.AddressId == addressId);
            var Product = db.Products.Single(P => P.ProductId == productId);

            var bvm = new BuyViewModel
            {
                Product = Product,
                address = address,
                AddressBook = db.Addresses.ToList(),
                States = db.States.ToList(),
                PaymentModes = db.Payments.ToList(),
                Order = Order

            };

            if (!ModelState.IsValid)
            {
                return View("CheckOut", bvm);

            }

            var user = db.AspNetUsers.Single(a => a.Id == id);
            var payment = db.Payments.Single(p => p.PaymentId == Order.PaymentId);
            Order.CreatedDate = DateTime.Now;
            Order.PaymentId = payment.PaymentId;
            Order.Id = user.Id;
            Order.Subtotal = Order.ItemQuantity * Product.Price;
            Order.ShippingAddress = address.HouseNo + "\n" + address.Colony_Street + "\n" +
                   address.City + "\n " + address.State.Name + "\n " + address.Pincode;
            //   var  address = add;

           
            if (!ModelState.IsValid)
            {

                return View("CheckOut", bvm);

            }
            else
            {
                if (Product.NumberOfStock == 0)
                {
                    ModelState.AddModelError("Order.ItemQuantity", "Out Of Stock");
                    return View("CheckOut", bvm);

                }

                if (Product.NumberOfStock < Order.ItemQuantity)
                {
                    
                   ModelState.AddModelError("Order.ItemQuantity", "Only " + Product.NumberOfStock + " stock is left");

                    return View("CheckOut", bvm);
                }
            }

            db.Orders.Add(Order);

            var existingcustomer = db.Customers.SingleOrDefault(c => c.Id == user.Id);
            if (existingcustomer == null)
            {
                Customer customer = new Customer()
                {
                    Name = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    CreatedDate = DateTime.Now,
                    CreatedBy = user.Id,
                    Id = user.Id

                };

                db.Customers.Add(customer);
            }

            
            db.SaveChanges();
            

             db.OrderDetails.Add(new OrderDetail
                {

                    ProductId = Product.ProductId,
                    OrderId = Order.OrderId,
                    Quantity = Order.ItemQuantity ,
                    TotalPrice = Product.Price * Order.ItemQuantity

                });

            Product.NumberOfStock = Product.NumberOfStock - Order.ItemQuantity;     
               
                db.SaveChanges();

            var email = new CartController();
             email.SendEmail(Order);

            return View(Order);
        }



        //[Authorize]
        //public ActionResult SelectShippingAddress(int Id)
        //{
        //    var product = db.Products.Single(p => p.ProductId == Id);
        //    string id = User.Identity.GetUserId();
        //    var user = db.AspNetUsers.Single(u => u.Id == id);
        //    var svm = new ShippingViewModel
        //    {
        //        User = user,
        //        Product = product,
        //        Addresses = db.Addresses.Where(u=>u.AspNetUser.Id==id).ToList(),
        //        States = db.States.ToList()

        //    };

        //    return View(svm);
        //}


        public ActionResult EditShippingAddress(int id, int productid)
        {
            var address = db.Addresses.Single(a => a.AddressId == id);
            var product = db.Products.Single(p => p.ProductId == productid);
            var bvm = new BuyViewModel
            {
                Product = product,
                address = address,
                AddressBook = db.Addresses.ToList(),
                States = db.States.ToList(),
                PaymentModes = db.Payments.ToList(),
                

            };
            return View(bvm);
        }

        
        //public ActionResult SelectPaymentMode(ShippingViewModel svm)
        //{
        //    var ovm =  new OrderViewModel();
        //    if (svm.ShippingAddress == null)
        //    {
        //        svm.NewAddress.Id = svm.User.Id;
        //        db.Addresses.Add(svm.NewAddress);
        //        db.SaveChanges();
        //        ovm.ShippingAddress = svm.NewAddress;

        //    }
        //    else
        //    {
        //      var   oldshippingaddress = db.Addresses.Single(a => a.AddressId == svm.ShippingAddress.AddressId);
        //        oldshippingaddress.City = svm.ShippingAddress.City;
        //        oldshippingaddress.Colony_Street = svm.ShippingAddress.Colony_Street;
        //        oldshippingaddress.HouseNo = svm.ShippingAddress.HouseNo;
        //        oldshippingaddress.Pincode = svm.ShippingAddress.Pincode;
        //        oldshippingaddress.StateId = svm.ShippingAddress.StateId;
        //        db.SaveChanges();
        //        ovm.ShippingAddress = oldshippingaddress;

        //    }
           
        //    var product = db.Products.Single(p => p.ProductId == svm.Product.ProductId);
        //    var user = db.AspNetUsers.Single(u => u.Id == svm.User.Id);
        //    ovm.Product = product;
        //    ovm.User = user;
        //    ovm.PaymentList = db.Payments.ToList();
        //    return View(ovm);
        //}

            
        
        public ActionResult GetProductByNameOrCategory(string searchkey, string name, string sortOrder, string currentFilter, string searchString, int? page)
       
        {
            ViewBag.searchkey = searchkey;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameDescParm = "name_desc";
            ViewBag.NameAsenParm = "name_asen";
            ViewBag.PriceAsenParm = "Price";
            ViewBag.PriceDescParm = "price_desc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var products = from s in db.SearchProcedure(searchkey).ToList()
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));

            }
            switch (sortOrder)
            {
                case "name_asen":

                    products = products.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    products = products.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(s => s.Price);
                    break;
                default:  // Name ascending 
                    products = products.OrderBy(s => s.Name);
                    break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));

        }
    }
      
    
}