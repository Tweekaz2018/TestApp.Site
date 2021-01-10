using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestApp.Services.Interfaces;

namespace TestApp.Integration_Tests.Helpers
{
    public class TestISaveFile : ISaveFile
    {
        public async Task<string> SaveFile(IFormFile file, string rootPath)
        {
            return await Task.Run(() => "test.jpg");
        }
    }
}
