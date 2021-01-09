using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Services.Interfaces;
using TestApp.Site.Controllers;
using TestApp.Site.Models.Shop;
using TestApp.Tests.Helpers;

namespace TestApp.Tests.Controllers
{
    [TestClass]
    public class Shop_Tests
    {
        ShopController controller;
        List<Item> items;

        [TestInitialize]
        public void SetTests()
        {
            RepositoryHelper helper = new RepositoryHelper();
            var categories = helper.GetCategories();
            items = helper.GetItems();
            Mock<IItemService> itemService = new Mock<IItemService>();
            itemService.Setup(x => x.GetCategories())
                .ReturnsAsync(() => categories.ToList());
            itemService.Setup(x => x.GetItems(0))
                .ReturnsAsync(items);
            itemService.Setup(x => x.GetItems(It.IsAny<int>()))
                .ReturnsAsync((int catId) => items.Where(x => x.CategoryId == catId).ToList());
            controller = new ShopController(itemService.Object);
        }

        [TestMethod]
        public async Task Index_Without_Category_Test()
        {
            var result = await controller.Index(null);

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<ShopMainModel>(viewResult.ViewData.Model);
            Assert.AreEqual("All candies", model.title);
            Xunit.Assert.IsType<List<Item>>(model.items);
            Xunit.Assert.IsType<List<Category>>(model.Categories);
            Assert.IsNotNull(model.Categories);
        }
        [TestMethod]
        public async Task Index_With_Category_Test()
        {
            var result = await controller.Index(2);

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<ShopMainModel>(viewResult.ViewData.Model);
            Assert.AreNotEqual("All candies", model.title);
            Xunit.Assert.IsType<List<Item>>(model.items);
            Xunit.Assert.IsType<List<Category>>(model.Categories);
            Assert.IsNotNull(model.Categories); 
            Assert.IsNotNull(model.items);
            Assert.AreNotEqual(items.Count, model.items.Count());
        }
        [TestMethod]
        public async Task Index_With_Category_which_not_exists_Test()
        {
            var result = await controller.Index(10);

            var viewResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
            Assert.AreEqual("Index", viewResult.ActionName);
        }
    }
}
