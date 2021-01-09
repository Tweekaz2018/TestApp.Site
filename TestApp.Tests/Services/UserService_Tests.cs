using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UserService_Tests
    {
        Mock<IRepository<User>> usersRepo;
        Mock<IRepository<UserRole>> userRolesRepo;
        Mock<IRepository<Cart>> cartsRepo;
        Mock<IRepository<CartItem>> cartItemsRepo;
        Mock<ISecurityService> securityService;
        Mock<IRepository<Order>> ordersRepo;
        Mock<IRepository<OrderItem>> orderItemsRepo;
        IUserService service;
        Context db;

        [TestInitialize]
        public void SetTests()
        {
            db = new Context(RepositoryHelper.TestDbContextOptions());
            RepositoryHelper.InitializeDbForTests(db);
            RepositoryHelper helper = new RepositoryHelper();
            usersRepo = helper.GetMockedRepo<User>(db);
            userRolesRepo = helper.GetMockedRepo<UserRole>(db);
            cartsRepo = helper.GetMockedRepo<Cart>(db);
            cartItemsRepo = helper.GetMockedRepo<CartItem>(db);
            ordersRepo = helper.GetMockedRepo<Order>(db);
            orderItemsRepo = helper.GetMockedRepo<OrderItem>(db);

            securityService = new Mock<ISecurityService>();
            securityService.Setup(x => x.CheckPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string password, string hashedPassword) =>
            {
                if (password == hashedPassword)
                    return true;
                return false;
            });
            securityService.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns((string pass) =>
                {
                    char[] charArray = pass.ToCharArray();
                    Array.Reverse(charArray);
                    return new string(charArray);
                });
            service = new UserService(usersRepo.Object, userRolesRepo.Object, cartsRepo.Object, cartItemsRepo.Object, ordersRepo.Object, orderItemsRepo.Object, securityService.Object);

        }

        [TestMethod]
        public async Task ChangeUserRole_Test()
        {
            int userId = 1;
            int newRole = 1;
            User user1 = await service.GetUser(userId);
            await service.ChangeUserRole(userId, newRole);
            User user2 = await service.GetUser(userId);

            usersRepo.Verify(x => x.UpdateAsync(It.IsAny<User>()));
            Assert.AreEqual(newRole, user2.UserRoleId);
        }

        [TestMethod]
        public async Task CheckUserPassword_Test()
        {
            string password = "admin";

            var result = await service.CheckUserPassword("Admin", password);

            usersRepo.Verify(x => x.GetRepo());
            securityService.Verify(x => x.CheckPassword(password, It.IsAny<string>()));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task RegisterUser_with_user_who_exists_Test()
        {
            User user = new User()
            {
                login = "Admin",
                password = "add",
                phone = "123123123"
            };

            var id = await service.RegisterUser(user);
        }

        [TestMethod]
        public async Task RegisterUser_with_who_not_exist_Test()
        {
            User user = new User()
            {
                login = "test user",
                password = "add",
                phone = "123123123"
            };

            var id = await service.RegisterUser(user);

            securityService.Verify(x => x.HashPassword(It.IsAny<string>()));
            usersRepo.Verify(x => x.AddAsync(It.IsAny<User>()));
        }

        [TestMethod]
        public async Task DeleteUser_Test()
        {
            int ordersCount = ordersRepo.Object.GetRepo().Count();
            int usersCount = usersRepo.Object.GetRepo().Count();

            User user = await service.GetUser(1);
            await service.DeleteUser(user);

            int ordersCountAfter = ordersRepo.Object.GetRepo().Count();
            int usersCountAfter = usersRepo.Object.GetRepo().Count();

            Assert.AreNotEqual(ordersCount, ordersCountAfter);
            Assert.AreNotEqual(usersCount, usersCountAfter);
            cartsRepo.Verify(x => x.GetRepo());
            ordersRepo.Verify(x => x.GetRepo());
        }
    }
}