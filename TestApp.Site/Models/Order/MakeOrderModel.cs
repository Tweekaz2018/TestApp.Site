using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TestApp.Domain;

namespace TestApp.Site.Models.Order
{
    public class MakeOrderModel
    {
        [Required]
        public int orderDeliveryMethod { get; set; }
        [Required]
        public int orderPayMethod { get; set; }

        public User user;
        public Domain.Cart cart;
        public IEnumerable<OrderDeliveryMethod> deliveryMethods;
        public IEnumerable<OrderPayMethod> payMethods;
    }
}
