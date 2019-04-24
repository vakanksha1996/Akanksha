using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Akanksha
{
    [MetadataType(typeof(SubcategoryMetaData))]
    public  partial class Subcategory
    {
    }

    public class SubcategoryMetaData
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required(ErrorMessage ="Choose Department")]
       public int? CategoryId { get; set; }
    
    }
}