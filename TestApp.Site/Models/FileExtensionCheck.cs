using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp.Site.Models
{
    public class FileExtensionCheck : ValidationAttribute
    {
        string[] _extensions;
        public FileExtensionCheck(string extensions)
        {
            var temp = extensions.Trim().Split(',');
            for(int i = 0; i < temp.Count(); i++)
            {
                if(temp[i].StartsWith('.') == false)
                {
                    temp[i] = $".{temp[i]}";
                }
            }
            _extensions = temp;
        }

        public override bool IsValid(object value)
        {
            if(value is IFormFile file)
            {
                string extension = Path.GetExtension(file.FileName);
                if (_extensions.Contains(extension.ToLower()) == false)
                    return false;
            }
            return true;
        }
    }
}
