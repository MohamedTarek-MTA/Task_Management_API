using System.ComponentModel.DataAnnotations;
using Task_Management_API.Domain.Enums;

namespace Task_Management_API.Application.DTOs.UserDTOs
{
    public class CreateUserDTO
    {
        [Required]
        public string FullName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public Role Role { get; set; }

    }
}
