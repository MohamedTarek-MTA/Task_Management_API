using Task_Management_API.Domain.Enums;

namespace Task_Management_API.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskItemStatus Status { get; set; }
        public TaskPriority Priority   { get; set; }
        public int ProjectId { get; set; }
        public int? AssignedUserId { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt {  get; set; }
        public DateTime? CompletedAt {  get; set; }
    }
}
