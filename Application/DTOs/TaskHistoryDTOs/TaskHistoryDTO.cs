using System.ComponentModel.DataAnnotations;

namespace Task_Management_API.Application.DTOs.TaskHistoryDTOs
{
    public class TaskHistoryDTO
    {
        public Guid Id { get; set; } 
        public Guid TaskItemId { get; set; }
        public string Action { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
