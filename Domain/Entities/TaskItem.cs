using System.ComponentModel.DataAnnotations;
using Task_Management_API.Domain.Enums;

namespace Task_Management_API.Domain.Entities
{
    public class TaskItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required,MaxLength(200)]
        public string Title { get; set; }
        [Required,MaxLength(1000)]
        public string Description { get; set; }
        public TaskItemStatus Status { get; set; } = TaskItemStatus.TODO;
        [Required]
        public TaskPriority Priority   { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt {  get; set; } 
        public DateTime? CompletedAt {  get; set; }
        [Required]
        public Guid ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public Guid? AssignedUserId { get; set; }
        public virtual User? AssignedUser { get; set; }

        public virtual ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
    }
}
