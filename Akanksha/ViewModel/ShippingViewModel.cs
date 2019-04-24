 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Akanksha.ViewModel
{
    public class ShippingViewModel
    {
        public Product Product { get; set; }
        public Address ShippingAddress { get; set; }
        //for address book
        public List<Address> Addresses { get; set; }
        public Address NewAddress { get; set; }
        public List<State> States { get; set; }
        public AspNetUser User { get; set; }
    }
}