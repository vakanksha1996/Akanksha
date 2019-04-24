using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Akanksha
{
    [MetadataType(typeof(CategoryMetaData))]
    public partial class Category
    {

    }


    public class CategoryMetaData
    {
        [Required(ErrorMessage ="Name is Required.")]
        public string Name { get; set; }
        public string Pic { get; set; }
        [Required]
        public string Description { get; set; }

    }
}