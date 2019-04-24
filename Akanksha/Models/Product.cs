using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Akanksha
{
    [MetadataType(typeof(ProductMetaData))]

    public partial  class Product
    {

    }

    public class ProductMetaData
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
       [Required(ErrorMessage = "Choose Category")]
        public int SubcategoryId { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public long NumberOfStock { get; set; }


    }
}