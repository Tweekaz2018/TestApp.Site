using System;
using System.Collections.Generic;
using System.Text;

namespace TestApp.Services.Interfaces
{
    public interface ISecurityService
    {
        byte[] GetSalt();
        string HashPassword(string password);
        bool CheckPassword(string password, string hashedPassword);
    }
}
