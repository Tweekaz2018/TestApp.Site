using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Services.Interfaces;

namespace TestApp.Services.Services
{
    public class SaveFileService : ISaveFile
    {
        public async Task<string> SaveFile(IFormFile file, string path)
        {
            string fileName = file.FileName;
            string absolutePath = path + fileName;
            if (File.Exists(absolutePath))
            {
                string fileType = file.FileName.Split('.').Last();
                fileName = $"{Guid.NewGuid().ToString()}.{fileType}";
                absolutePath = path + fileName;
            }
            using (var fileStream = new FileStream(absolutePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return fileName;
        }
    }
}