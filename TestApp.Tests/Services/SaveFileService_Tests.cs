using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestApp.Services.Interfaces;
using TestApp.Services.Services;

namespace TestApp.Tests.Services
{
    [TestClass]
    public class SaveFileService_Tests
    {
        ISaveFile _service;
        Mock<IFormFile> mockFormFile;
        [TestInitialize]
        public void SetTests()
        {
            _service = new SaveFileService();
        }
        [TestMethod]
        public async Task SaveFile_Test()
        {
            string fileName = "carousel1.jpg";
            mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(x => x.FileName)
                .Returns(fileName);
            mockFormFile.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()));

            string resultFileName = await _service.SaveFile(mockFormFile.Object,AppContext.BaseDirectory);

            Assert.AreNotEqual(fileName, resultFileName);
            mockFormFile.Verify(x => x.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()));
        }
    }
}
