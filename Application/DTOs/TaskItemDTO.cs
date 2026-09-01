using System.ComponentModel.DataAnnotations;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Domain.Enums;

namespace Task_Management_API.Application.DTOs
{
    public class TaskItemDTO
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public TaskItemStatus Status { get; set; } = TaskItemStatus.TODO;
        [Required]
        public TaskPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }
        [Required]
        public Guid ProjectId { get; set; }
        public Guid? AssignedUserId { get; set; }
    }
}
