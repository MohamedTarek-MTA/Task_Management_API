using Riok.Mapperly.Abstractions;
using Task_Management_API.Application.DTOs.ProjectDTOs;
using Task_Management_API.Domain.Entities;

namespace Task_Management_API.Application.Mappers
{
    [Mapper]
    public partial class ProjectMapper
    {
        public partial ProjectDTO ToDTO(Project project);
        public partial Project ToEntity(ProjectDTO projectDTO);
        public partial CreateProjectDTO ToCreationDTO(Project project);
        public partial Project ToEntity(CreateProjectDTO createProjectDTO);

        public partial UpdateProjectDTO ToUpdateDTO(Project project);

        public partial void Map(UpdateProjectDTO projectDTO, [MappingTarget] Project project);
    }
}
