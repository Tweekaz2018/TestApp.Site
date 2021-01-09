using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp.Site.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Login")]
        [StringLength(128, MinimumLength = 5, ErrorMessage = "Login must be more than 5 and less than 128 symbols")]
        [Remote(action: "CheckLogin", controller: "Login", ErrorMessage = "Login is in use now")]
        public string login { get; set; }
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Password must be more than 8 symbol")]
        public string password { get; set; }
        [Required]
        [Phone]
        public string phone { get; set; }
    }
}
