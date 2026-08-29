using Riok.Mapperly.Abstractions;
using Task_Management_API.Application.DTOs;
using Task_Management_API.Domain.Entities;

namespace Task_Management_API.Application.Mappers
{
    [Mapper]
    public partial class ProjectMapper
    {
        public partial ProjectDTO ToDTO(Project project);
        public partial Project ToEntity(ProjectDTO projectDTO);
    }
}
