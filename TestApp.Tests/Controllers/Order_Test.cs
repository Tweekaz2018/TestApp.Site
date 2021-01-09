using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Services.Interfaces;
using TestApp.Site.Controllers;
using TestApp.Site.Models.Order;
using TestApp.Tests.Helpers;

namespace TestApp.Tests.Controllers
{
    [TestClass]
    public class Order_Test
    {
        OrderController controller;
        Mock<IOrderService> _orderService;
        Mock<IUserService> _userService;
        Mock<ICartService> _cartService;
        List<User> users;
        List<Cart> carts;
        List<Order> orders;
        List<OrderDeliveryMethod> orderDeliveryMethods;
        List<OrderPayMethod> orderPayMethods;
        List<CartItem> cartItems;
        List<OrderItem> orderItems;

        [TestInitialize]
        public void SetTests()
        {
            RepositoryHelper helper = new RepositoryHelper();
            users = helper.GetUsers();
            carts = helper.GetCarts();
            orders = helper.GetOrders();
            cartItems = helper.GetCartItems();
            orderItems = helper.GetOrderItems();
            orderDeliveryMethods = helper.GetOrderDeliveryMethods();
            orderPayMethods = helper.GetOrderPayMethods();
            _orderService = new Mock<IOrderService>();
            _orderService.Setup(x => x.GetOrderPayMethods())
                .ReturnsAsync(orderPayMethods);
            _orderService.Setup(x => x.GetOrderDeliveryMethods())
                .ReturnsAsync(orderDeliveryMethods);
            _orderService.Setup(x=>x.CreateOrder(It.IsAny<Order>(), It.IsAny<int>()))
                .ReturnsAsync((Order order, int cartId)=>{
                    var orderId = orders.Max(x => x.Id) + 1;
                    List<CartItem> items = new List<CartItem>();
                    foreach (var item in items)
                        order.OrderItems.Add(new OrderItem()
                        {
                            ItemId = item.ItemId,
                            OrderId = orderId,
                            quantity = item.quantity
                        });
                    order.Id = orderId;
                    orders.Add(order);
                    return orderId;
            });
            _orderService.Setup(x => x.GetOrderDetailed(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    Order order = orders.SingleOrDefault(x => x.Id == id);
                    if(order != null)
                    {
                        order.OrderDeliveryMethod = orderDeliveryMethods.SingleOrDefault(x => x.Id == order.OrderDeliveryMethodId);
                        order.OrderPayMethod = orderPayMethods.SingleOrDefault(x => x.Id == order.OrderPayMethodId);
                        order.OrderItems = orderItems.Where(x => x.OrderId == id).ToList();
                    }
                    return order;
                });
            _userService = new Mock<IUserService>();
            _userService.Setup(x => x.GetUser(It.IsAny<int>()))
                .ReturnsAsync((int id) => users.SingleOrDefault(x => x.Id == id));
            _cartService = new Mock<ICartService>();
            _cartService.Setup(x => x.GetCart(It.IsAny<int>()))
                .ReturnsAsync((int id) => carts.SingleOrDefault(x => x.Id == id));
            controller = new OrderController(_orderService.Object, _cartService.Object, _userService.Object);
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, "2"),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, "User")
                    }, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType));
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        [TestMethod]
        public async Task MakeOrder_Page_Test()
        {
            var result = await controller.MakeOrder();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<MakeOrderModel>(viewResult.ViewData.Model);
            Assert.IsNotNull(model.cart);
            Assert.IsNotNull(model.user);
            Assert.IsNotNull(model.orderDeliveryMethod);
            Assert.IsNotNull(model.orderPayMethod);
        }

        [TestMethod]
        public async Task MakeOrder_Post_Valid_Test()
        {
            MakeOrderModel model = new MakeOrderModel()
            {
                orderDeliveryMethod = 1,
                orderPayMethod = 1
            };

            var result = await controller.MakeOrder(model);
            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            Assert.IsNotNull(viewResult.ViewData["orderId"]);
            Assert.AreEqual(viewResult.ViewName, "OrderSuccess");
        }

        [TestMethod]
        public async Task GetOrderDetails_Not_Valid_User_For_This_Order__Test()
        {
            int orderId = 1;

            var result = await controller.GetOrderDetails(orderId);

            Xunit.Assert.IsType<RedirectResult>(result);
        }

        [TestMethod]
        public async Task GetOrderDetails_Valid_Test()
        {
            int orderId = 1;
            var user = new ClaimsPrincipal(
               new ClaimsIdentity(
                   new Claim[] {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, "1"),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, "Admin")
                   }, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType));
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            var result = await controller.GetOrderDetails(orderId);

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<OrderDetailsModalModel>(viewResult.ViewData.Model);
            Assert.IsNotNull(model.order);
        }


    }
}
