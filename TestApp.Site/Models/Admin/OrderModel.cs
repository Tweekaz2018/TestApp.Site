using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Domain;

namespace TestApp.Site.Models.Admin
{
    public class ItemModel
    {
        [Required]
        [Display(Name = "Title")]
        public string title { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string description { get; set; }
        [Required]
        [Display(Name = "Price")]
        public double price { get; set; }
        [Required]
        [Display(Name = "Category")]
        public int categoryId { get; set; }
        [Display(Name = "Image")]
        [FileExtensions(Extensions = "jpg,jpeg,png")]
        public IFormFile image { get; set; }

        public IEnumerable<Category> categories { get; set; }

    }
}
