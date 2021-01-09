using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApp.Site.Models
{
    public class ErrorModel
    {
        public string errorMessage { get; set; }
        public ErrorModel(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }
    }
}
