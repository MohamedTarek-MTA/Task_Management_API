namespace Task_Management_API.Domain.Entities
{
    public class TaskHistory
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public string Action { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
