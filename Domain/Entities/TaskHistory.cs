using System.ComponentModel.DataAnnotations;

namespace Task_Management_API.Domain.Entities
{
    public class TaskHistory
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid TaskItemId { get; set; }
        [Required , MaxLength(100)]
        public string Action { get; set; }
        [MaxLength(500)]
        public string? OldValue { get; set; }
        [MaxLength(500)]
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
