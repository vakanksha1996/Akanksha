using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Akanksha.ViewModel;
using System.Web.Http;

namespace Akanksha.Controllers
{
    [System.Web.Mvc.Authorize]
    public class NormalUserController : Controller
    {
        private AkankshaEntities db;

        public NormalUserController()
        {
            db = new AkankshaEntities();
        }



        // GET: NormalUser
        public ActionResult Index()
        {
            return View();
        }


        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Account()
        {
            string id = User.Identity.GetUserId();
            var user = db.AspNetUsers.SingleOrDefault(c => c.Id == id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                AspNetUser new_user = new AspNetUser();
                return View(new_user);
            }
        }

        
        public ActionResult AddPayment()
        {
            string id = User.Identity.GetUserId();
            var customer = db.Customers.Single(c => c.Id == id);
            return View(customer);
        }

        public ActionResult AddAddress()
        {
            var avm = new AddressViewModel()
            {
                 states = db.States.ToList()
        };
            
            return View(avm);
        }


        public ActionResult EditAddress(int id)
        {
            var avm = new AddressViewModel()
            {
                address = db.Addresses.Single(a => a.AddressId == id),
                states = db.States.ToList()

            };
            return View(avm);
        }

        public ActionResult SaveAddress(Address address)
        {
            if (address.AddressId == 0)
            {
                address.Id = User.Identity.GetUserId();
               db.Addresses.Add(address);
                
                
            }
            else
            {
                var addressInDb = db.Addresses.Single(a => a.AddressId == address.AddressId);
                addressInDb.City = address.City;
                addressInDb.HouseNo = address.HouseNo;
                addressInDb.Colony_Street = address.Colony_Street;
                addressInDb.StateId = address.StateId;
                addressInDb.Pincode = address.Pincode;
               


            }
            db.SaveChanges();
            return RedirectToAction("YourAddresses");
        }


        public ActionResult YourAddresses()
        {
            string id = User.Identity.GetUserId();
            var addressbook = db.Addresses.Where(a => a.Id == id).ToList();
            return View(addressbook);
        }




        public ActionResult EditProfile()
        {
            string id = User.Identity.GetUserId();
            var user = db.AspNetUsers.Single(u => u.Id == id);

            return View(user);
        }



        public ActionResult SaveProfile(AspNetUser user)
        {
            if (user != null)
            {
                var userInDb = db.AspNetUsers.Single(u => u.Id == user.Id);
              //  userInDb.Email = user.Email;
                userInDb.PhoneNumber = user.PhoneNumber;
                userInDb.UserName = user.UserName;
                userInDb.Email = user.UserName;
                db.SaveChanges();
                return View("Account",user);
            }
            return RedirectToAction("EditProfile",user.Id);
        }

        public JsonResult DeleteAddress(int Id)
        {
            var address= db.Addresses.Single(p => p.AddressId == Id);
            if (address == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
            db.Addresses.Remove(address);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [System.Web.Http.HttpPost]
        public JsonResult  SavePaymentCard(string UserId,string ccn)
        {

            if (UserId == null || ccn == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

               var  CustomerinDb = db.Customers.Single(c => c.Id == UserId);
                CustomerinDb.CreditCardNumber =ccn;
               
                 db.SaveChanges();
                 return   Json(true,JsonRequestBehavior.AllowGet);
           

        }
    }
}