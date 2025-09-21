using System.ComponentModel.DataAnnotations;

namespace EventTicketing.API.DTOs
{
    /// <summary>
    /// DTO para el registro de usuarios
    /// </summary>
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }
    }

    /// <summary>
    /// DTO para el inicio de sesión
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; }
    }

    /// <summary>
    /// DTO para el cambio de contraseña
    /// </summary>
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "La contraseña actual es requerida")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no coinciden")]
        public string ConfirmPassword { get; set; }
    }
}