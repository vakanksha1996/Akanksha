using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Akanksha.ViewModel
{
    public class OrderViewModel
    {
        public Address ShippingAddress { get; set; }
        public Product Product { get; set; }
        public AspNetUser User { get; set; }
        public List<Payment> PaymentList { get; set; }
        public Order Order { get; set; }
    }
}