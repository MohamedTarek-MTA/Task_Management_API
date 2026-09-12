using System.ComponentModel.DataAnnotations;
using Task_Management_API.Domain.Enums;

namespace Task_Management_API.Application.DTOs.UserDTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        
        public string FullName { get; set; }
       
        public string  Email { get; set; }
        
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
