using System;
using System.Security.Cryptography;
using EventTicketing.Core.Interfaces;

namespace EventTicketing.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de hash de contraseñas usando BCrypt
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // En una implementación real, se usaría BCrypt.Net-Next u otra biblioteca
            // Esta es una implementación simulada para fines educativos
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            // En una implementación real, se usaría BCrypt.Net-Next u otra biblioteca
            // Esta es una implementación simulada para fines educativos
            byte[] hashBytes = Convert.FromBase64String(passwordHash);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }

            return true;
        }
    }
}