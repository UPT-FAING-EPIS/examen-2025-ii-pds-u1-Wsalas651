using System;
using System.ComponentModel.DataAnnotations;

namespace EventTicketing.API.DTOs
{
    /// <summary>
    /// DTO para mostrar información básica de un usuario
    /// </summary>
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }

    /// <summary>
    /// DTO para mostrar el perfil completo de un usuario
    /// </summary>
    public class UserProfileDto : UserDto
    {
        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// DTO para actualizar el perfil de un usuario
    /// </summary>
    public class UpdateUserProfileDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// DTO para actualizar el rol de un usuario
    /// </summary>
    public class UpdateUserRoleDto
    {
        [Required(ErrorMessage = "El rol es requerido")]
        public string Role { get; set; }
    }
}