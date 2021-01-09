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
using TestApp.Tests.Helpers;

namespace TestApp.Tests.Controllers
{
    [TestClass]
    public class Cart_Tests
    {
        CartController controller;
        List<Cart> carts;
        List<CartItem> cartItems;
        Mock<ICartService> cartService;
        [TestInitialize]
        public void SetTests()
        {
            RepositoryHelper helper = new RepositoryHelper();
            carts = helper.GetCarts();
            cartItems = helper.GetCartItems();
            cartService = new Mock<ICartService>();
            cartService.Setup(x => x.GetCart(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    Cart cart = carts.SingleOrDefault(x => x.Id == id);
                    cart.CartItems = cartItems.Where(x => x.CartId == id).ToList();
                    return cart;
                });
            cartService.Setup(x => x.AddItemToCart(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
            cartService.Setup(x => x.DeleteFromCart(It.IsAny<int>(), It.IsAny<int>()));
            controller = new CartController(cartService.Object);
            var user = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] {
                        new Claim(ClaimsIdentity.DefaultNameClaimType, "1"),
                        new Claim(ClaimsIdentity.DefaultRoleClaimType, "Admin")
                    }, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType));
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        [TestMethod]
        public async Task GetCart_Test()
        {
            var result = await controller.Index();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<Cart>(viewResult.ViewData.Model);
            Assert.IsNotNull(model.CartItems);
        }

        [TestMethod]
        public void GetAddItemToCartModal_Test()
        {
            int id = 1;
            var result = controller.AddItemToCart(id);

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            Assert.AreEqual(id, viewResult.ViewData["ItemId"]);
        }

        [TestMethod]
        public async Task AddItemToCart_Save_Test()
        {
            int itemId = 1;
            int quantity = 10;

            await controller.AddItemToCart(itemId, quantity);

            cartService.Verify(x => x.AddItemToCart(itemId, It.IsAny<int>(), quantity));
        }

        [TestMethod]
        public async Task DeleteItemFromCart_Test()
        {
            int cartId = 1;
            int itemId = 1;

            var result = await controller.RemoveItemFromCart(cartId, itemId);

            cartService.Verify(x => x.DeleteFromCart(itemId, cartId));
            var viewResult = Xunit.Assert.IsType<RedirectResult>(result);
            Assert.AreEqual("/cart/", viewResult.Url);
        }
        [TestMethod]
        public async Task DivideItemFromCart_Test()
        {
            int cartId = 1;
            int itemId = 1;

            var result = await controller.DevideItemIncart(cartId, itemId);

            cartService.Verify(x => x.DevideItemInCart(itemId, cartId));
            var viewResult = Xunit.Assert.IsType<RedirectResult>(result);
            Assert.AreEqual("/cart/", viewResult.Url);
        }
        [TestMethod]
        public async Task SupplementItemInCart_Test()
        {
            int cartId = 1;
            int itemId = 1;

            var result = await controller.SupplementItemInCart(cartId, itemId);

            cartService.Verify(x => x.SupplementItemInCart(itemId, cartId));
            var viewResult = Xunit.Assert.IsType<RedirectResult>(result);
            Assert.AreEqual("/cart/", viewResult.Url);
        }

    }
}
