using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTicketing.Core.Domain;

namespace EventTicketing.Core.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de usuarios
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        Task<User> RegisterUserAsync(string email, string password, string firstName, string lastName, string phoneNumber, UserRole role = UserRole.Customer);

        /// <summary>
        /// Autentica a un usuario
        /// </summary>
        Task<(bool Success, User User, string Token)> AuthenticateAsync(string email, string password);

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        Task<User> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// Actualiza el perfil de un usuario
        /// </summary>
        Task<User> UpdateUserProfileAsync(Guid userId, string firstName, string lastName, string phoneNumber);

        /// <summary>
        /// Cambia la contraseña de un usuario
        /// </summary>
        Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Obtiene usuarios por rol
        /// </summary>
        Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);

        /// <summary>
        /// Asigna un rol a un usuario
        /// </summary>
        Task AssignRoleToUserAsync(Guid userId, UserRole role);
    }
}