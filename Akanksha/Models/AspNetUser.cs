using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Akanksha
{
    [MetadataType(typeof(AspNetUserMetaData))]
    public partial class AspNetUser
    {

    }


    public class AspNetUserMetaData
    {
        [Required(ErrorMessage = "EmailId is required.")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="PhoneNumber is required")]
        public string PhoneNumber { get; set; }

    }
}