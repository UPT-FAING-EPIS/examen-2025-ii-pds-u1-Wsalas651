using System;

namespace EventTicketing.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de hash de contraseñas
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Genera un hash para una contraseña
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si una contraseña coincide con un hash
        /// </summary>
        bool VerifyPassword(string password, string passwordHash);
    }
}