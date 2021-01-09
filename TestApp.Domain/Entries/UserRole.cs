using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Domain
{
    public class UserRole : Entity
    {
        public string name { get; set; }
        public List<User> Users { get; set; }
    }
}
