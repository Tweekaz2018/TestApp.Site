using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain
{
    public class OrderDeliveryMethod : Entity
    {
        public string name { get; set; }
        public List<Order> Orders { get; set; }
    }
}
