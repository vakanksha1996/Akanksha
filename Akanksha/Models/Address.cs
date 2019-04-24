using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace Akanksha
{
   [MetadataType(typeof(AddressMetaData))]
    public partial class Address
    {

    }
    public class AddressMetaData
    {
        [Required]
        [Display(Name = "HouseNo or Flat No")]
        public string HouseNo { get; set; }
        [Required]
        [Display(Name = "Street")]
        public string Colony_Street { get; set; }
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }
        [Required]
        [Display(Name = "Pincode")]
        public string Pincode { get; set; }
        [Required]
        [Display(Name = "State")]
        public int  StateId { get; set; }


    }
}