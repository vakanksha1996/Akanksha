using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Akanksha.ViewModel
{
    public class AddressViewModel
    {
        public Address address { get; set; }
        public List<State> states { get; set; }
    }
}