using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Task_Management_API.Domain.Entities
{
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required, MaxLength(150)]
        public string FullName { get; set; }
        [Required]
        public string Role { get; set; }
        [Required,EmailAddress, MaxLength(200)]
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    }
}
