using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Akanksha.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<Subcategory> SubcategoryList { get; set; }
    }
}