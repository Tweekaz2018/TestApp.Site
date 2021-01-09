using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Services.Interfaces;
using TestApp.Site.Controllers;
using TestApp.Site.Models;
using TestApp.Site.Models.Admin;
using TestApp.Tests.Helpers;

namespace TestApp.Tests.Controllers
{
    [TestClass]
    public class Admin_Tests
    {
        AdminController controller;
        Mock<IOrderService> _orderService;
        Mock<IItemService> _itemService;
        Mock<IUserService> _userService;
        Mock<IWebHostEnvironment> _env;
        Mock<ISaveFile> _saveFileService;
        int pageSize;

        List<Item> items;
        List<Category> categories;
        List<User> users;
        List<Order> orders;
        List<OrderDeliveryMethod> orderDeliveryMethods;
        List<OrderPayMethod> orderPayMethods;
        

        [TestInitialize]
        public void SetTests()
        {
            RepositoryHelper helper = new RepositoryHelper();
            items = helper.GetItems();
            categories = helper.GetCategories();
            users = helper.GetUsers();
            orders = helper.GetOrders();
            orderDeliveryMethods = helper.GetOrderDeliveryMethods();
            orderPayMethods = helper.GetOrderPayMethods();

            pageSize = 2;

            _orderService = new Mock<IOrderService>();
            _orderService.Setup(x => x.OrdersCount())
                .ReturnsAsync(orders.Count);
            _orderService.Setup(x => x.GetOrders(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((int skip, int take) => orders.Skip(skip).Take(take));
            _orderService.Setup(x => x.GetOrderDeliveryMethods())
                .ReturnsAsync(orderDeliveryMethods);
            _orderService.Setup(x => x.GetOrderPayMethods())
                .ReturnsAsync(orderPayMethods);
            _orderService.Setup(x => x.AddDeliveryMethod(It.IsAny<OrderDeliveryMethod>()))
                .ReturnsAsync((OrderDeliveryMethod odm) =>
                {
                    var newId = orderDeliveryMethods.Max(x => x.Id) + 1;
                    odm.Id = newId;
                    orderDeliveryMethods.Add(odm);
                    return newId;
                });
            _orderService.Setup(x => x.UpdateDeliveryMethod(It.IsAny<OrderDeliveryMethod>()))
                .Returns(async (OrderDeliveryMethod odm) =>
                {
                    var order = orderDeliveryMethods.SingleOrDefault(x => x.Id == odm.Id);
                    order.name = odm.name;
                    await Task.CompletedTask;
                });
            _orderService.Setup(x => x.RemoveDeliveryMethod(It.IsAny<int>()))
                .Returns(async (int id) =>
                {
                    var odm = orderDeliveryMethods.SingleOrDefault(x => x.Id == id);
                    orderDeliveryMethods.Remove(odm);
                    await Task.CompletedTask;
                });
            _orderService.Setup(x => x.AddPayMethod(It.IsAny<OrderPayMethod>()))
                .ReturnsAsync((OrderPayMethod opm) =>
                {
                    var newId = orderPayMethods.Max(x => x.Id) + 1;
                    opm.Id = newId;
                    orderPayMethods.Add(opm);
                    return newId;
                });
            _orderService.Setup(x => x.UpdatePayMethod(It.IsAny<OrderPayMethod>()))
                .Returns(async (OrderPayMethod opm) =>
                {
                    var order = orderPayMethods.SingleOrDefault(x => x.Id == opm.Id);
                    order.name = opm.name;
                    await Task.CompletedTask;
                });
            _orderService.Setup(x => x.RemovePayMethod(It.IsAny<int>()))
                .Returns(async (int id) =>
                {
                    var opm = orderPayMethods.SingleOrDefault(x => x.Id == id);
                    orderPayMethods.Remove(opm);
                    await Task.CompletedTask;
                });
            _itemService = new Mock<IItemService>();
            _itemService.Setup(x => x.GetCategories())
                .ReturnsAsync(categories);
            _itemService.Setup(x => x.ItemsCount())
                .ReturnsAsync(items.Count);
            _itemService.Setup(x => x.GetItems(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((int skip, int take) =>
                    items.Skip(skip).Take(take).ToList()
                    );
            _itemService.Setup(x => x.AddItem(It.IsAny<Item>()))
                .ReturnsAsync((Item item) =>
                {
                    int newId = items.Max(x => x.Id) + 1;
                    item.Id = newId;
                    items.Add(item);
                    return newId;
                });
            _itemService.Setup(x => x.DeleteItem(It.IsAny<int>()));
            _itemService.Setup(x => x.AddCategory(It.IsAny<Category>()))
                .ReturnsAsync((Category cat) =>
                {
                    var newId = categories.Max(x => x.Id) + 1;
                    cat.Id = newId;
                    categories.Add(cat);
                    return newId;
                });
            _itemService.Setup(x => x.UpdateCategory(It.IsAny<Category>()))
                .Returns(async (Category category) =>
                {
                    var cat = categories.SingleOrDefault(x => x.Id == category.Id);
                    cat.name = category.name;
                    await Task.CompletedTask;
                });
            _itemService.Setup(x => x.DeleteCategory(It.IsAny<int>()));
            _userService = new Mock<IUserService>();
            _userService.Setup(x => x.DeleteUser(It.IsAny<User>()));
            _userService.Setup(x => x.UsersCount())
                .ReturnsAsync(users.Count);
            _userService.Setup(x => x.GetUsers(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((int skip, int take) => users.Skip(skip).Take(take).ToList());
            _env = new Mock<IWebHostEnvironment>();
            _env.Setup(x => x.WebRootPath)
                .Returns("//");
            _saveFileService = new Mock<ISaveFile>();
            _saveFileService.Setup(x => x.SaveFile(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ReturnsAsync((IFormFile file, string path) => file.FileName);
            controller = new AdminController(_itemService.Object, _orderService.Object, _userService.Object, _env.Object, _saveFileService.Object, pageSize);
        }

        [TestMethod]
        public void GetIndex_Page_Test()
        {
            var result = controller.Index();

            Xunit.Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public async Task GetItemsPage_Test()
        {
            var result = await controller.GetItemsPage();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<PageViewModel<Item>>(viewResult.ViewData.Model);
            Assert.AreEqual(pageSize, model.list.Count());
            Assert.AreEqual(true, model.HasNextPage);
        }
        [TestMethod]
        public async Task AddItem_Modal_Test()
        {
            var result = await controller.GetItemAddModal();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<ItemModel>(viewResult.ViewData.Model);
            Assert.AreEqual(categories.Count, model.categories.Count());
        }

        [TestMethod]
        public async Task AddItem_Post_Test()
        {
            Mock<IFormFile> mockIFormFile = new Mock<IFormFile>();
            mockIFormFile.Setup(x => x.FileName)
                .Returns(Guid.NewGuid().ToString());
            mockIFormFile.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()));
            ItemModel model = new ItemModel()
            {
                categoryId = 1,
                price = 10,
                title = "title",
                description = "qweqe",
                image = mockIFormFile.Object
            };

            var result = await controller.AddItem(model);

            var viewResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
            Assert.AreEqual("GetItemsPage", viewResult.ActionName); _saveFileService.Verify(x => x.SaveFile(It.IsAny<IFormFile>(), It.IsAny<string>()));
            _itemService.Verify(x => x.AddItem(It.IsAny<Item>()));
        }

        [TestMethod]
        public async Task DeleteItem_Test()
        {
            int id = 1;
            var result = await controller.DeleteItem(id);
            var viewResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
            Assert.AreEqual("GetItemsPage", viewResult.ActionName);
            _itemService.Verify(x => x.DeleteItem(id));
        }

        [TestMethod]
        public async Task GetUsersPage_Test()
        {
            var result = await controller.GetUsersPage(1);

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<PageViewModel<User>>(viewResult.ViewData.Model);
            Assert.AreEqual(pageSize, model.list.Count());
        }

        [TestMethod]
        public async Task Delete_User_Test()
        {
            var result = await controller.DeleteUser(1);

            var viewResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
            Assert.AreEqual("GetUsersPage", viewResult.ActionName);
        }

        [TestMethod]
        public async Task GetOrdersPage_Test()
        {
            var result = await controller.GetOrdersPage(1);

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<PageViewModel<Order>>(viewResult.ViewData.Model);
            Assert.AreEqual(pageSize, model.list.Count());
        }

        [TestMethod]
        public async Task GetOrdersPage_Page_Is_Not_Valid_Test()
        {
            var result = await controller.GetOrdersPage(1000);

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<PageViewModel<Order>>(viewResult.ViewData.Model);
            Assert.AreEqual(0, model.list.Count());
        }

        [TestMethod]
        public async Task GetOrdersDeliveryMethodsPage_Test()
        {
            var result = await controller.GetOrdersDeliveryMethodsPage();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<PageViewModel<OrderDeliveryMethod>>(viewResult.ViewData.Model);
            Assert.AreEqual(pageSize, model.list.Count());
        }

        [TestMethod]
        public void AddDeliveryMethodModal_Test()
        {
            var result = controller.AddDeliveryMethodModal();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public async Task AddDeliveryMethod_Test()
        {
            var result = await controller.AddDeliveryMethod("New order delivery");

            var viewResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
            Assert.AreEqual("GetOrdersDeliveryMethodsPage", viewResult.ActionName);
        }

        [TestMethod]
        public async Task UpdateDeliveryMethod_Test()
        {
            var odm = orderDeliveryMethods.First();
            string newName = "new name";

            await controller.UpdateDeliveryMethod(newName, odm.Id);

            var odmAfterUpdate = orderDeliveryMethods.SingleOrDefault(x => x.name == newName);
            Assert.IsNotNull(odmAfterUpdate);            
        }

        [TestMethod]
        public async Task DeleteDeliveryMethod_Test()
        {
            var odm = orderDeliveryMethods.First();

            await controller.DeleteDeliveryMethod(odm.Id);

            var needToBeNullNow = orderDeliveryMethods.SingleOrDefault(x => x.Id == odm.Id);
            Assert.IsNull(needToBeNullNow);
        }

        [TestMethod]
        public async Task GetOrdersPayMethodsPage_Test()
        {
            var result = await controller.GetOrdersPayMethodsPage();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<PageViewModel<OrderPayMethod>>(viewResult.ViewData.Model);
            Assert.AreEqual(pageSize, model.list.Count());
        }

        [TestMethod]
        public void AddPayMethodModal_Test()
        {
            var result = controller.AddPayMethodModal();

            Xunit.Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public async Task AddPayMethod_Test()
        {
            var opmCount = orderPayMethods.Count;
            string newName = "new name";

            var result = await controller.AddPayMethod(newName);

            var viewResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
            Assert.AreEqual("GetOrdersPayMethodsPage", viewResult.ActionName);
            Assert.AreNotEqual(opmCount, orderPayMethods.Count);
        }

        [TestMethod]
        public async Task UpdatePayMethod_Test()
        {
            var opm = orderPayMethods.First();
            string newName = "new name";

            await controller.UpdatePayMethod(newName, opm.Id);

            var needToBeNotNull = orderPayMethods.SingleOrDefault(x => x.name == newName);
            Assert.IsNotNull(needToBeNotNull);
        }

        [TestMethod]
        public async Task DeletePayMethod_Test()
        {
            var opm = orderPayMethods.First();
            var opmCount = orderPayMethods.Count;

            await controller.DeletePayMethod(opm.Id);

            var afterDelete = orderPayMethods.Count;
            Assert.AreNotEqual(opmCount, afterDelete);
        }

        [TestMethod]
        public async Task GetItemsCategories_Test()
        {
            var result = await controller.GetItemsCategories();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<PageViewModel<Category>>(viewResult.ViewData.Model);
            Assert.AreEqual(pageSize, model.list.Count());
        }

        [TestMethod]
        public void AddCategoryModal_Test()
        {
            var result = controller.AddCategoryModal();

            Xunit.Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public async Task AddCategory_Test()
        {
            var catCount = categories.Count;
            string newName = "new name";

            var result = await controller.AddCategory(newName);

            var viewResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
            Assert.AreEqual("GetItemsCategories", viewResult.ActionName);
            Assert.AreNotEqual(catCount, categories.Count);
        }

        [TestMethod]
        public async Task UpdateCategory_Test()
        {
            var cat = categories.First();
            string newName = "new name";

            await controller.UpdateCategory(newName, cat.Id);

            var needToBeNotNull = categories.SingleOrDefault(x => x.name == newName);
            Assert.IsNotNull(needToBeNotNull);
        }

        [TestMethod]
        public async Task DeleteCategory_Test()
        {
            var cat = categories.First();
            var catCount = categories.Count;

            await controller.DeleteCategory(cat.Id);

            _itemService.Verify(x => x.DeleteCategory(cat.Id));
        }

    }
}
