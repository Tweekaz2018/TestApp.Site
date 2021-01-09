using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
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
using TestApp.Site.Models;
using TestApp.Tests.Helpers;

namespace TestApp.Tests.Controllers
{
    [TestClass]
    public class Login_Tests
    {
        LoginController controller;
        Mock<IUserService> userService;
        List<User> users;

        [TestInitialize]
        public void SetTests()
        {
            RepositoryHelper helper = new RepositoryHelper();
            users = helper.GetUsers();
            userService = new Mock<IUserService>();
            userService.Setup(x => x.CheckUserPassword(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string login, string password) =>
                {
                    var user = users.SingleOrDefault(x => x.login == login);
                    if (user == null)
                        return false;
                    if (user.password == password)
                        return true;
                    return false;
                });
            userService.Setup(x => x.GetUserByLogin(It.IsAny<string>()))
                .ReturnsAsync((string login) =>
                    users.SingleOrDefault(x => x.login == login));
            userService.Setup(x => x.CheckLogin(It.IsAny<string>()))
            .ReturnsAsync((string login) => users.Select(x => x.login).Contains(login));
            userService.Setup(x => x.RegisterUser(It.IsAny<User>()))
                .ReturnsAsync((User user) =>
                {
                    var newId = users.Max(x => x.Id) + 1;
                    user.Id = newId;
                    users.Add(user);
                    user.UserRole = new UserRole()
                    {
                        name = "Users"
                    };
                    return newId;
                });
            userService.Setup(x => x.GetUser(It.IsAny<int>()))
                .ReturnsAsync((int id) => users.SingleOrDefault(x => x.Id == id));
            controller = new LoginController(userService.Object);
        }

        [TestMethod]
        public void Login_Page_Test()
        {
            var result = controller.Login();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        }
        [TestMethod]
        public async Task Login_Valid_Test()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);
            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IUrlHelperFactory)))
                .Returns(urlHelperFactory.Object);

            var controller = new LoginController(userService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProviderMock.Object
                    }
                }
            };
            LoginModel model = new LoginModel()
            {
                login = "Admin",
                password = "nimda"
            };

            var result = await controller.Login(model);

            var viewResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
            Assert.AreEqual("Index", viewResult.ActionName);
            authServiceMock.Verify(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));
        }
        [TestMethod]
        public async Task Login_Not_Valid_Test()
        {
            LoginModel model = new LoginModel()
            {
                login = "Admin",
                password = "asdadad"
            };

            var result = await controller.Login(model);

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            Xunit.Assert.IsType<LoginModel>(viewResult.Model);

        }

        [TestMethod]
        public void Register_Page_Test()
        {
            var result = controller.Register();

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public async Task CheckLogin_Valid_Test()
        {
            var result = await controller.CheckLogin("Admin");

            var viewResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.IsType<bool>(viewResult.Value);
            Assert.AreEqual(true, viewResult.Value);
        }

        [TestMethod]
        public async Task CheckLogin_Non_Valid_Test()
        {
            var result = await controller.CheckLogin("asdsadasd");

            var viewResult = Xunit.Assert.IsType<JsonResult>(result);
            Xunit.Assert.IsType<bool>(viewResult.Value);
            Assert.AreEqual(false, viewResult.Value);
        }

        [TestMethod]
        public async Task Register_Valid_Test()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);
            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IUrlHelperFactory)))
                .Returns(urlHelperFactory.Object);

            var controller = new LoginController(userService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProviderMock.Object
                    }
                }
            };
            RegisterModel model = new RegisterModel()
            {
                login = "logiin",
                password = "paaaaaass",
                phone = "123213213"
            };
            var result = await controller.Register(model);
            var viewResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
            Assert.AreEqual(viewResult.ActionName, "Index");
            authServiceMock.Verify(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()));

        }
        [TestMethod]
        public async Task Register_NonValidModel_Test()
        {
            RegisterModel model = new RegisterModel()
            {
                login = "login",
                password = "123",
                phone = "asda"
            };

            var result = await controller.Register(model);

            var viewResult = Xunit.Assert.IsType<ViewResult>(result);
            Xunit.Assert.IsType<RegisterModel>(viewResult.ViewData.Model);
        }
        [TestMethod]
        public async Task Logout_Test()
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);
            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            serviceProviderMock
                .Setup(s => s.GetService(typeof(IUrlHelperFactory)))
                .Returns(urlHelperFactory.Object);
            var controller = new LoginController(userService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProviderMock.Object
                    }
                }
            };

            var result = await controller.Logout();

            var viewResult = Xunit.Assert.IsType<RedirectToActionResult>(result);
            Assert.AreEqual("Index", viewResult.ActionName);
            authServiceMock.Verify(x => x.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()));
        }
    }
}
