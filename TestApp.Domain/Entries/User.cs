using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TestApp.Domain
{
    public class User : Entity
    {
        public string login { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string phone { get; set; }
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }
        public Cart Cart { get; set; }
        public List<Order> Orders { get; set; }
    }
}
