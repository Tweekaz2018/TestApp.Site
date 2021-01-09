using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain
{
    public class OrderItem : Entity
    {
        public int ItemId { get; set; }
        public Item item { get; set; }
        public int quantity { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
