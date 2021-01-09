using System;
using System.ComponentModel.DataAnnotations;

namespace TestApp.Domain
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
    }
}
