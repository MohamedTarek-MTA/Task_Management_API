using System.ComponentModel.DataAnnotations;
using Task_Management_API.Domain.Enums;

namespace Task_Management_API.Application.DTOs.ProjectDTOs
{
    public class CreateProjectDTO
    {
        [Required,MaxLength(100)]
        public string Name { get; set; }
        [Required, MaxLength(1000)]
        public string Description { get; set; }
        [Required]
        public DateTime StartDate { get; set; } 
        public DateTime? EndDate { get; set; }
        [Required]
        public ProjectStatus ProjectStatus { get; set; }
    }
}
