using EventTicketing.Core.Domain;

namespace EventTicketing.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el generador de tokens JWT
    /// </summary>
    public interface IJwtTokenGenerator
    {
        /// <summary>
        /// Genera un token JWT para un usuario
        /// </summary>
        string GenerateToken(User user);
    }
}