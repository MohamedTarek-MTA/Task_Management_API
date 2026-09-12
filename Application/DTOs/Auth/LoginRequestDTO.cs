using System.ComponentModel.DataAnnotations;

namespace Task_Management_API.Application.DTOs.Auth
{
    public class LoginRequestDTO
    {
        [Required, EmailAddress, MaxLength(200)]
        public string Email { get; set; } = null!;
        [Required, MinLength(8), MaxLength(100), RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } = null!;
    }
}
