using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Domain;

namespace TestApp.Site.Models.Shop
{
    public class ShopMainModel
    {
        public string title { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Item> items { get; set; }
    }
}
