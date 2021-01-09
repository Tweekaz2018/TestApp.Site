using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestApp.Domain;
using TestApp.Services.Interfaces;
using TestApp.Services.Services;
using TestApp.Site.Controllers;
using TestApp.Site.Models.Home;

namespace TestApp.Tests.Controllers
{
    [TestClass]
    public class Home_Tests
    {
        HomeController controller;

        [TestInitialize]
        public void SetTests()
        {
            Mock<IUserService> userService = new Mock<IUserService>();
            userService.Setup(x => x.GetUserWithOrder(1))
                .ReturnsAsync((int id) => new User() { Id = 1 });
            Mock<IOrderService> orderService = new Mock<IOrderService>();

            controller = new HomeController(userService.Object, orderService.Object);
        }

        [TestMethod]
        public void Index_Test()
        {
            var result = controller.Index();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            Assert.IsNull(viewResult.ViewData.Model);
        }

        [TestMethod]
        public async Task Profile_Logged_Test()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimsIdentity.DefaultNameClaimType, "1"),
                                        new Claim(ClaimsIdentity.DefaultRoleClaimType, "Admin")
                                        // other required and custom claims
                                   }, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType));
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
            var result = await controller.Profile();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            var model = Xunit.Assert.IsType<UserProfileModel>(viewResult.Model);
            Assert.AreEqual(1, model.user.Id);
        }
    }
}