using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Services.Interfaces
{//Выделил в отдельный, чтобы потом, если нужно будет добавить что-то типа аватарок юзерам - просто переиспользовать
    public interface ISaveFile
    {
        Task<string> SaveFile(IFormFile file, string rootPath);
    }
}
