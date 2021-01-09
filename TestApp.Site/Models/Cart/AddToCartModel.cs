using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp.Site.Models
{
    public class AddToCartModel
    {
        [Required]
        public int ItemId { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public int quantity { get; set; }
    }
}
