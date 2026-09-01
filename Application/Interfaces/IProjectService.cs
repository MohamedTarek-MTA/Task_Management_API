using Task_Management_API.Application.DTOs;
using Task_Management_API.Domain.Enums;
using X.PagedList;

namespace Task_Management_API.Application.Interfaces
{
    public interface IProjectService
    {
        public Task<ProjectDTO> GetProjectById(Guid id);
        public Task<IEnumerable<ProjectDTO>> GetAllProjects();
        public Task<IPagedList<ProjectDTO>> GetAllProjectsPaged(int pageNumber, int pageSize);
        public Task<ProjectDTO> CreateProject(ProjectDTO projectDTO);
        public Task<ProjectDTO> UpdateProject(Guid id, ProjectDTO projectDTO);
        public Task DeleteProject(Guid id);
        public Task<IPagedList<ProjectDTO>> FindProjectsByName(string name, int pageNumber, int pageSize);
        public Task<IPagedList<ProjectDTO>> FindProjectsByStatus(ProjectStatus status, int pageNumber, int pageSize);
        public Task<ProjectDTO> ChangeProjectStatus(Guid id, ProjectStatus newStatus);
    }
}
