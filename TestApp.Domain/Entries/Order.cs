using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain
{
    public class Order : Entity
    {
        public DateTime DateCreate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public int OrderDeliveryMethodId { get; set; }
        public OrderDeliveryMethod OrderDeliveryMethod { get; set; }
        public int OrderPayMethodId { get; set; }
        public OrderPayMethod OrderPayMethod { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
