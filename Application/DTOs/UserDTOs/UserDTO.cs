using System.ComponentModel.DataAnnotations;

namespace Task_Management_API.Application.DTOs.UserDTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required, EmailAddress]
        public string  Email { get; set; }
        [Required]
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
