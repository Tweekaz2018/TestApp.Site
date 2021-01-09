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
    public class OrderService_Tests
    {
        Mock<IRepository<Order>> ordersRepo;
        Mock<IRepository<OrderDeliveryMethod>> orderDeliveryMethodsRepo;
        Mock<IRepository<OrderPayMethod>> orderPayMethodsRepo;
        Mock<IRepository<OrderItem>> orderItemsRepo;
        Mock<IRepository<Cart>> cartsRepo;
        Mock<IRepository<CartItem>> cartItemsRepo;
        IOrderService service;

        Context db;

        [TestInitialize]
        public void SetTests()
        {
            db = new Context(RepositoryHelper.TestDbContextOptions());
            RepositoryHelper.InitializeDbForTests(db);
            RepositoryHelper helper = new RepositoryHelper();
            ordersRepo = helper.GetMockedRepo<Order>(db);
            orderDeliveryMethodsRepo = helper.GetMockedRepo<OrderDeliveryMethod>(db);
            orderPayMethodsRepo = helper.GetMockedRepo<OrderPayMethod>(db);
            orderItemsRepo = helper.GetMockedRepo<OrderItem>(db);
            cartsRepo = helper.GetMockedRepo<Cart>(db);
            cartItemsRepo = helper.GetMockedRepo<CartItem>(db);

            service = new OrderService(ordersRepo.Object, orderDeliveryMethodsRepo.Object, orderPayMethodsRepo.Object, orderItemsRepo.Object, cartsRepo.Object, cartItemsRepo.Object);
        }

        [TestMethod]
        public async Task Create_and_Get_Order_Test()
        {
            Order order = new Order()
            {
                OrderDeliveryMethodId = 1,
                OrderPayMethodId = 1,
                UserId = 1
            };

            int orderId = await service.CreateOrder(order, 1);
            Order savedOrderWithDetails = await service.GetOrderDetailed(orderId);

            Assert.IsNotNull(savedOrderWithDetails.OrderItems);
        }
    }
}
