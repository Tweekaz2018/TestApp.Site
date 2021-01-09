using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain
{
    public class CartItem : Entity
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }
        public int quantity { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
