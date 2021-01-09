using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApp.Domain;
using TestApp.Services.Interfaces;
using TestApp.Site.Models.Order;

namespace TestApp.Site.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {

        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, ICartService cartService, IUserService userService)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userService = userService;
        }
        public async Task<IActionResult> MakeOrder()
        {
            int cartId = int.Parse(User.Identity.Name);
            User user = await _userService.GetUser(cartId);
            Cart cart = await _cartService.GetCart(cartId);
            IEnumerable<OrderDeliveryMethod> orderDeliveryMethods = await _orderService.GetOrderDeliveryMethods();
            IEnumerable<OrderPayMethod> orderPayMethods = await _orderService.GetOrderPayMethods();
            return View(new MakeOrderModel()
            {
                cart = cart,
                user = user,
                deliveryMethods = orderDeliveryMethods,
                payMethods = orderPayMethods
            });
        }

        [HttpPost]
        public async Task<IActionResult> MakeOrder(MakeOrderModel order)
        {
            int cartId = int.Parse(User.Identity.Name);
            Order newOrder = new Order()
            {
                DateCreate = DateTime.Now,
                OrderDeliveryMethodId = order.orderDeliveryMethod,
                OrderPayMethodId = order.orderPayMethod,
                UserId = cartId
            };
            int orderId = await _orderService.CreateOrder(newOrder, cartId);
            ViewBag.orderId = orderId;
            return View("OrderSuccess");
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            Order order = await _orderService.GetOrderDetailed(id);
            int userId = int.Parse(User.Identity.Name);
            if (!User.IsInRole("Admin") || order.UserId != userId)
                return Redirect("//");
            OrderDetailsModalModel model = new OrderDetailsModalModel()
            {
                order = order
            };
            return View(model);
        }
    }
}
