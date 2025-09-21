using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTicketing.Core.Domain;

namespace EventTicketing.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el repositorio de usuarios
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Obtiene un usuario por su email
        /// </summary>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Obtiene usuarios por rol
        /// </summary>
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);

        /// <summary>
        /// Verifica si un email ya está registrado
        /// </summary>
        Task<bool> IsEmailRegisteredAsync(string email);

        /// <summary>
        /// Obtiene un usuario con sus tickets comprados
        /// </summary>
        Task<User> GetUserWithTicketsAsync(Guid userId);
    }
}