using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain
{
    public class Item : Entity
    {
        public string title { get; set; }
        public string description { get; set; }
        public string imagePath { get; set; }
        public double price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
