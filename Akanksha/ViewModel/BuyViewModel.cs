using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Akanksha.ViewModel
{
    public class BuyViewModel
    {
        public Order Order { get; set; }
        public Product Product { get; set; }
        public Address address { get; set; }
        public List<Address> AddressBook { get; set; }
        public List<State> States { get; set; }
        public List<Payment> PaymentModes { get; set; }
    }
}