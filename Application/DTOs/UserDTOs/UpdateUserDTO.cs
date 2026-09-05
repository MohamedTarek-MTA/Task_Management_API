using System.ComponentModel.DataAnnotations;

namespace Task_Management_API.Application.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        public Guid? Id { get; set; }
       
        public string? FullName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
