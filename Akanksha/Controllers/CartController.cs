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
using System.Net.Mail;
using System.Net;

namespace Akanksha.Controllers
{
    public class CartController : Controller
    {
        private AkankshaEntities db;


        public CartController()
        {
            db = new AkankshaEntities();
        }


        // GET: Cart
        public ActionResult Index()
        {
            
            return View();
        }



        public ActionResult GetCartItemList()
        {
            string Id = User.Identity.GetUserId();
            var cart_items = db.Carts.Where(c => c.Id == Id);
            return View(cart_items);
        }



       [Authorize]
        public JsonResult AddtoCart(int productid)
        {
            string id = User.Identity.GetUserId();
            var product = db.Products.Single(p => p.ProductId == productid);
            var isalreadyadded =    db.Carts.Count(c => c.ProductId == product.ProductId && c.Id == id);
            if (isalreadyadded == 0)
            {
                Cart c = new Cart
                {
                    Id = User.Identity.GetUserId(),
                    ProductId = productid,
                    Quantity = 1,
                    Amount = product.Price

                };
                db.Carts.Add(c);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
           
           // return RedirectToAction("GetProductsByName","Product", new { name = product.Subcategory.Name });
        }




        public JsonResult RemovefromCart(int id)
            
        {
            var cart = db.Carts.SingleOrDefault(c => c.CartId == id);
            if (cart != null)
            {
                db.Carts.Remove(cart);
                db.SaveChanges();
                string Id = User.Identity.GetUserId();
                var cart_items = db.Carts.Where(c => c.Id == Id);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);

        }





        public JsonResult UpdateQuantity(int cartid,int qty)
        {

            var cart = db.Carts.Single(c => c.CartId == cartid);
            if (cart != null)
            {
                cart.Quantity = qty;
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }




        public ActionResult CheckOut(Address address)
        {
            string id = User.Identity.GetUserId();
         
            var cvm = new CheckOutViewModel
            {
                Cartitems = db.Carts.Where(c => c.Id == id && c.Product.NumberOfStock!=0).ToList(),
                States = db.States.ToList(),
                PaymentModes = db.Payments.ToList(),
                AddressBook = db.Addresses.Where(a=>a.Id == id).ToList()
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

            if (address.City != null && address.AddressId==0)
            {
                address.Id = User.Identity.GetUserId();
                db.Addresses.Add(address);
                db.SaveChanges();
                cvm.address = address;
            }
            return View(cvm);
        }



        public ActionResult EditShippingAddress(int id)
        {
            var address = db.Addresses.Single(a => a.AddressId == id);
            var avm = new AddressViewModel
            {
                address = address,
                states = db.States.ToList()
            };
            return View(avm);
        }



        public ActionResult OrderCompletion(int AddressId, Order Order) 
        {
            string id = User.Identity.GetUserId();
            var address = db.Addresses.Single(a => a.AddressId == AddressId);
            var cartitems = db.Carts.Where(c => c.Id == id && c.Product.NumberOfStock != 0).ToList();

            var cm = new CheckOutViewModel
            {
                Cartitems = cartitems,
                address = address,
                AddressBook = db.Addresses.ToList(),
                States = db.States.ToList(),
                PaymentModes = db.Payments.ToList(),
                Order = Order

            };

            if (!ModelState.IsValid)
            {
                return View("CheckOut", cm);
            }

            var user = db.AspNetUsers.Single(a => a.Id == id);
            var payment = db.Payments.Single(p => p.PaymentId == Order.PaymentId);
           
          
           
            Order.CreatedDate = DateTime.Now;
            Order.PaymentId = payment.PaymentId;
            Order.Id = user.Id;
            Order.ShippingAddress = address.HouseNo + "\n" + address.Colony_Street + "\n" +
                   address.City + "\n " + address.State.Name + "\n " + address.Pincode;

          
           

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
        

            foreach (var item in cartitems)
            {
                item.PaymentDate = DateTime.Now;
                db.OrderDetails.Add(new OrderDetail
                {

                    ProductId = item.ProductId,
                    OrderId = Order.OrderId,
                    Quantity = item.Quantity,
                    TotalPrice = item.Product.Price*item.Quantity,
                    
                });
               
                var producttobebuy = db.Products.Single(p => p.ProductId == item.ProductId);
                if(producttobebuy.NumberOfStock < item.Quantity)
                {
                    Order.ItemQuantity = Order.ItemQuantity - item.Quantity;
                    Order.Subtotal = Order.Subtotal - item.Quantity * item.Product.Price;
                    db.SaveChanges();
                    return Content(producttobebuy.Name + " is out of stock");
                }
                else
                {
                    producttobebuy.NumberOfStock = producttobebuy.NumberOfStock - item.Quantity;
                    db.SaveChanges();
                }
              
            }

            SendEmail(Order);                 
            return View(Order);
        }


        public void SendEmail(Order order)
        {
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("0d641e048c0b8d", "8a0f3c27bd6ab2"),
                EnableSsl = true
            };

            MailMessage mail = new MailMessage();
              mail.To.Add(order.AspNetUser.Email);
            mail.From = new MailAddress("admin@shop.com");
            mail.Subject = "Order Successfully Placed!!";
            string Body = "Hii..."+order.AspNetUser.Email + ",</br></br>Thank you for Shopping.<br> Your Order Id:" + order.OrderId + ".</br> Order Date: " + order.CreatedDate;
            mail.Body = Body;
            mail.IsBodyHtml = true;
            client.Send(mail);
            Console.WriteLine("Sent");
            //Email obj = new Email
            //{
            //    Body = "Hi " + order.AspNetUser.Email + ",</br></br>Thank you for Shopping.<br> Your Order Id:" + order.OrderId + ".</br> Order Date: " + order.CreatedDate,
            //    From = "admin@BookMyTicket.com",
            //    Subject = "Booking Confirmation mail",
            //    To = b.Email
            //};
            //  Session["moviename"] = null;


            //if (ModelState.IsValid)
            //{
            //    MailMessage mail = new MailMessage();
            //    mail.To.Add(obj.To);
            //    mail.From = new MailAddress(obj.From);
            //    mail.Subject = obj.Subject;
            //    string Body = obj.Body;
            //    mail.Body = Body;
            //    mail.IsBodyHtml = true;
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = "	smtp.mailtrap.io";
            //    smtp.Port = 25;
            //    smtp.UseDefaultCredentials = false;
            //    smtp.Credentials = new System.Net.NetworkCredential("0d641e048c0b8d", "8a0f3c27bd6ab2"); // Enter seders User name and password  
            //    smtp.EnableSsl = true;
            //    smtp.Send(mail);
            //    //  return View("Index", obj);
            //}
        }

    }
}