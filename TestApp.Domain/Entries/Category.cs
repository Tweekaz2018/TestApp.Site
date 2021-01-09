using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain
{
    public class Category : Entity
    {
        public string name { get; set; }
        public List<Item> Items { get; set; }
    }
}
