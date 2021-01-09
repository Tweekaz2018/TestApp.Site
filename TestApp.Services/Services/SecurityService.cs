using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Text;
using TestApp.Services.Interfaces;

namespace TestApp.Services.Services
{
    public class SecurityService : ISecurityService
    {
        public byte[] GetSalt() => Encoding.ASCII.GetBytes("testing");
        public bool CheckPassword(string password, string hashedPassword)
        {
            var hashedInputedPass = HashPassword(password);
            if (hashedInputedPass == hashedPassword)
                return true;
            return false;
        }

        public string HashPassword(string password)
        {
            var salt = GetSalt();
            return Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8
                    )
                );
        }
    }
}
