using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Akanksha.Models
{
    public class SubcategoryViewModel
    {
      
        public Subcategory SubCategory { get; set; }
        public List<Category> DepartmentList { get; set; }
        public List<Subcategory> CategoryList { get; set; }
    }
}