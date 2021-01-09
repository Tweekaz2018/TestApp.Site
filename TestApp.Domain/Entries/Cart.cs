using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain
{
    public class Cart : Entity
    {
        public List<CartItem> CartItems { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
