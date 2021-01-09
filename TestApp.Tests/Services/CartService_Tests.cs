using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Repo;
using TestApp.Services.Interfaces;
using TestApp.Services.Services;
using TestApp.Tests.Helpers;

namespace TestApp.Tests.Services
{
    [TestClass]
    public class CartService_Tests
    {
        ICartService service;

        Mock<IRepository<Cart>> _cartsRepo;
        Mock<IRepository<CartItem>> _cartItemsRepo;
        Mock<IRepository<Item>> _items;
        Context db;

        [TestInitialize]
        public void SetTests()
        {
            db = new Context(RepositoryHelper.TestDbContextOptions());
            RepositoryHelper.InitializeDbForTests(db);
            RepositoryHelper helper = new RepositoryHelper();
            _cartsRepo = helper.GetMockedRepo<Cart>(db);
            _cartItemsRepo = helper.GetMockedRepo<CartItem>(db);
            _items = helper.GetMockedRepo<Item>(db);

            service = new CartService(_cartsRepo.Object, _cartItemsRepo.Object, _items.Object);
        }


        [TestCleanup]
        public void DownTest()
        {
            db.Dispose();
        }

        [TestMethod]
        public async Task AddItemToCart_Test()
        {
            int cartId = 1;
            Cart cart1 = await service.GetCart(cartId);
            int cartItems = cart1.CartItems.Count;

            await service.AddItemToCart(1, cartId, 10);
            await service.AddItemToCart(3, cartId, 10);
            Cart cart2 = await service.GetCart(cartId);

            Assert.AreEqual(cartItems + 1, cart2.CartItems.Count);
            _cartItemsRepo.Verify(x => x.UpdateAsync(It.IsAny<CartItem>()));
        }

        [TestMethod]
        public async Task ClearCart_Test()
        {
            int cartId = 1;

            await service.ClearCart(cartId);
            Cart cart = await service.GetCart(cartId);

            Assert.AreEqual(0, cart.CartItems.Count);
        }
    }
}
