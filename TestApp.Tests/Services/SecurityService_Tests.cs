using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestApp.Services.Interfaces;
using TestApp.Services.Services;

namespace TestApp.Tests.Services
{
    [TestClass]
    public class SecurityService_Tests
    {
        ISecurityService service;

        [TestInitialize]
        public void SetTests()
        {
            service = new SecurityService();
        }

        [TestMethod]
        public void SecurityService_Test_Two_Diffent_Passwords_Are_Not_Equal_After_Hashing()
        {

            var hashedPassword1 = service.HashPassword("123456");
            var hashedPassword2 = service.HashPassword("1234567");

            Assert.AreNotEqual(hashedPassword1, hashedPassword2);
        }

        [TestMethod]
        public void SecurityService_Test_Two_Similar_Passwords_Are_Equal_After_Hashing()
        {
            var hashedPassword1 = service.HashPassword("1234a56");
            var hashedPassword2 = service.HashPassword("1234a56");

            Assert.AreEqual(hashedPassword1, hashedPassword2);
        }

        [TestMethod]
        public void SecurityService_Passoword_With_Mistake_Test()
        {
            var hashedPassword = service.HashPassword("123456");

            var PasswodWithMistake = service.CheckPassword("123!56", hashedPassword);

            Assert.IsFalse(PasswodWithMistake);

        }

        [TestMethod]
        public void SecurityService_Passoword_Check_Without_Mistake_Test()
        {
            var hashedPassword = service.HashPassword("123456");

            var PasswodWithoutMistake = service.CheckPassword("123456", hashedPassword);

            Assert.IsTrue(PasswodWithoutMistake);
        }

    }
}
