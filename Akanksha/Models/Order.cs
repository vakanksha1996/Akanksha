using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Akanksha
{
    [MetadataType(typeof(OrderMetaData))]
    public partial class Order
    {

    }

    public class OrderMetaData
    {
        [Display(Name = "Payment Mode")] 
        [Required(ErrorMessage= "Payment Mode is required.")]
         [Range(1,4,ErrorMessage ="Payment Mode is required.")]
         public int PaymentId { get; set; }


        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1,double.MaxValue ,ErrorMessage = "Quantity should be greater than 0.")]
        public int ItemQuantity { get; set; }

    }

}