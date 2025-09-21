using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventTicketing.Core.Domain;
using EventTicketing.Core.Interfaces;

namespace EventTicketing.Core.Application
{
    /// <summary>
    /// Implementación del servicio de usuarios
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator tokenGenerator)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _tokenGenerator = tokenGenerator ?? throw new ArgumentNullException(nameof(tokenGenerator));
        }

        public async Task<User> RegisterUserAsync(string email, string password, string firstName, string lastName, string phoneNumber, UserRole role = UserRole.Customer)
        {
            // Verificar que el email no está registrado
            var isEmailRegistered = await _userRepository.IsEmailRegisteredAsync(email);
            if (isEmailRegistered)
                throw new InvalidOperationException($"El email {email} ya está registrado");

            // Generar hash de la contraseña
            var passwordHash = _passwordHasher.HashPassword(password);

            // Crear el usuario
            var user = new User(email, passwordHash, firstName, lastName, phoneNumber, role);
            await _userRepository.AddAsync(user);

            return user;
        }

        public async Task<(bool Success, User User, string Token)> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return (false, null, null);

            // Verificar la contraseña
            var isPasswordValid = _passwordHasher.VerifyPassword(password, user.PasswordHash);
            if (!isPasswordValid)
                return (false, null, null);

            // Generar token JWT
            var token = _tokenGenerator.GenerateToken(user);

            return (true, user, token);
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");

            return user;
        }

        public async Task<User> UpdateUserProfileAsync(Guid userId, string firstName, string lastName, string phoneNumber)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");

            user.UpdateProfile(firstName, lastName, phoneNumber);
            await _userRepository.UpdateAsync(user);

            return user;
        }

        public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");

            // Verificar la contraseña actual
            var isCurrentPasswordValid = _passwordHasher.VerifyPassword(currentPassword, user.PasswordHash);
            if (!isCurrentPasswordValid)
                throw new InvalidOperationException("La contraseña actual es incorrecta");

            // Generar hash de la nueva contraseña
            var newPasswordHash = _passwordHasher.HashPassword(newPassword);

            user.ChangePassword(newPasswordHash);
            await _userRepository.UpdateAsync(user);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _userRepository.GetUsersByRoleAsync(role);
        }

        public async Task AssignRoleToUserAsync(Guid userId, UserRole role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");
        
            user.AssignRole(role);
            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdateUserRoleAsync(Guid userId, UserRole role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");
        
            user.AssignRole(role);
            await _userRepository.UpdateAsync(user);
        }
    }
}