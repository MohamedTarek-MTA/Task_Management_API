using System.ComponentModel.DataAnnotations;
using Task_Management_API.Domain.Enums;

namespace Task_Management_API.Application.DTOs
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime StartDate { get; set; }  = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.TODO;
    }
}
