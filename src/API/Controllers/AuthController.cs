using System;
using System.Threading.Tasks;
using EventTicketing.API.DTOs;
using EventTicketing.Core.Domain;
using EventTicketing.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventTicketing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userService.RegisterUserAsync(
                    registerDto.Email,
                    registerDto.Password,
                    registerDto.FirstName,
                    registerDto.LastName,
                    registerDto.PhoneNumber,
                    UserRole.Customer);

                return Ok(new { UserId = user.Id, Message = "Usuario registrado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Error interno del servidor" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.AuthenticateAsync(loginDto.Email, loginDto.Password);

                if (!result.Success)
                    return Unauthorized(new { Message = "Credenciales inválidas" });

                return Ok(new
                {
                    UserId = result.User.Id,
                    Email = result.User.Email,
                    FullName = result.User.GetFullName(),
                    Role = result.User.Role.ToString(),
                    Token = result.Token
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Error interno del servidor" });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = Guid.Parse(User.FindFirst("sub")?.Value);
                await _userService.ChangePasswordAsync(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

                return Ok(new { Message = "Contraseña cambiada exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Error interno del servidor" });
            }
        }
    }
}