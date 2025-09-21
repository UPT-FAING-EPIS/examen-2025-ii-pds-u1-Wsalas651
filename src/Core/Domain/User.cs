using System;
using System.Collections.Generic;

namespace EventTicketing.Core.Domain
{
    /// <summary>
    /// Representa un usuario del sistema
    /// </summary>
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string PhoneNumber { get; private set; }
        public DateTime RegistrationDate { get; private set; }
        public UserRole Role { get; private set; }
        public List<Ticket> Tickets { get; private set; } = new List<Ticket>();

        // Constructor privado para EF Core
        private User() { }

        // Constructor para crear un nuevo usuario
        public User(string email, string passwordHash, string firstName, string lastName, 
                  string phoneNumber, UserRole role = UserRole.Customer)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email no puede estar vacío", nameof(email));
            
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("El hash de contraseña no puede estar vacío", nameof(passwordHash));
            
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("El nombre no puede estar vacío", nameof(firstName));
            
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("El apellido no puede estar vacío", nameof(lastName));

            Id = Guid.NewGuid();
            Email = email.ToLower();
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            RegistrationDate = DateTime.Now;
            Role = role;
        }

        // Métodos de dominio
        public void UpdateProfile(string firstName, string lastName, string phoneNumber)
        {
            if (!string.IsNullOrWhiteSpace(firstName))
                FirstName = firstName;
            
            if (!string.IsNullOrWhiteSpace(lastName))
                LastName = lastName;
            
            if (!string.IsNullOrWhiteSpace(phoneNumber))
                PhoneNumber = phoneNumber;
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("El nuevo hash de contraseña no puede estar vacío", nameof(newPasswordHash));
            
            PasswordHash = newPasswordHash;
        }

        public void AssignRole(UserRole role)
        {
            Role = role;
        }

        public string GetFullName()
        {
            return $"{FirstName} {LastName}";
        }
    }

    public enum UserRole
    {
        Customer,
        Organizer,
        Administrator
    }
}