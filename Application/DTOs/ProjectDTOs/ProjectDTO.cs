using System.ComponentModel.DataAnnotations;
using Task_Management_API.Domain.Enums;

namespace Task_Management_API.Application.DTOs.ProjectDTOs
{
    public class ProjectDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime? EndDate { get; set; }
        public ProjectStatus ProjectStatus { get; set; } 
    }
}
