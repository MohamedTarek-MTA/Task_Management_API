using Microsoft.IdentityModel.Tokens;
using Task_Management_API.Application.DTOs;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Application.Mappers;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Domain.Enums;
using Task_Management_API.Infrastructure.Repositories;
using X.PagedList;
using X.PagedList.Extensions;

namespace Task_Management_API.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly Repository<Project> _repository;
        private readonly ProjectMapper _projectMapper;
        public ProjectService(ILogger<ProjectService> logger, Repository<Project> repository, ProjectMapper projectMapper)
        {
            _logger = logger;
            _repository = repository;
            _projectMapper = projectMapper;
        }
        public async Task<ProjectDTO> GetProjectById(Guid id)
        {
            var project = await _repository.GetByIdAsync(id);
            if (project == null)
            {
                _logger.LogWarning($"Project with ID {id} not found.");
                throw new KeyNotFoundException($"Project with ID {id} not found.");
            }
            return _projectMapper.ToDTO(project);
        }
        public async Task<IEnumerable<ProjectDTO>> GetAllProjects()
        {
            var projects = await _repository.GetAllAsync();
            if (projects.IsNullOrEmpty())
            {
                _logger.LogInformation("No projects found.");
                return [];
            }
            return projects
                .Select(project => _projectMapper.ToDTO(project))
                .OrderBy(project => project.Name);
        }
        public async Task<IPagedList<ProjectDTO>> GetAllProjectsAsync(int pageNumber, int pageSize)
        {
            var projects = await _repository.GetAllAsync();
            if (projects.IsNullOrEmpty())
            {
                _logger.LogInformation("No projects found.");
                return new PagedList<ProjectDTO>(new List<ProjectDTO>(), pageNumber, pageSize);
            }
            var projectDTOs = projects
                .Select(project => _projectMapper.ToDTO(project))
                .OrderBy(project => project.Name)
                .ToList();
            return new PagedList<ProjectDTO>(projectDTOs, pageNumber, pageSize);
        }
        public async Task<ProjectDTO> CreateProject(ProjectDTO projectDTO)
        {
            var project = _projectMapper.ToEntity(projectDTO);
            await _repository.AddAsync(project);
            return _projectMapper.ToDTO(project);
        }
        public async Task<ProjectDTO> UpdateProject(Guid id, ProjectDTO projectDTO)
        {
            var existingProject = await _repository.GetByIdAsync(id);
            if (existingProject == null)
            {
                _logger.LogWarning($"Project with ID {id} not found.");
                throw new KeyNotFoundException($"Project with ID {id} not found.");
            }
            var updatedProject = _projectMapper.ToEntity(projectDTO);
            updatedProject.Id = id;
            _repository.Update(updatedProject);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to update project.");
                throw new Exception("Failed to update project.");
            }
            return _projectMapper.ToDTO(updatedProject);

        }
        public async Task DeleteProject(Guid id)
        {
            var existingProject = await _repository.GetByIdAsync(id);
            if (existingProject == null)
            {
                _logger.LogWarning($"Project with ID {id} not found.");
                throw new KeyNotFoundException($"Project with ID {id} not found.");
            }
            foreach (var taskItem in existingProject.TaskItems)
            {
                if (!taskItem.Status.Equals(TaskItemStatus.COMPLETED) || !taskItem.Status.Equals(TaskItemStatus.CANCELLED))
                {
                    _logger.LogWarning($"Cannot delete project with ID {id} because it has incomplete tasks.");
                    throw new InvalidOperationException($"Cannot delete project with ID {id} because it has incomplete tasks.");
                }
            }
            _repository.Remove(existingProject);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to delete project.");
                throw new Exception("Failed to delete project.");
            }
        }
        public async Task<IPagedList<ProjectDTO>> FindProjectsByName(string name, int pageNumber, int pageSize)
        {
            var projects = await _repository.FindAsync(project => project.Name.Equals(name));
            if (projects.IsNullOrEmpty())
            {
                _logger.LogInformation("No projects found matching the condition.");
                return new StaticPagedList<ProjectDTO>(new List<ProjectDTO>(), pageNumber, pageSize, 0);
            }
            return projects
                .Select(project => _projectMapper.ToDTO(project))
                .OrderBy(project => project.Name)
                .ToPagedList();
        }

        public async Task<IPagedList<ProjectDTO>> FindProjectsByStatus(ProjectStatus status, int pageNumber, int pageSize)
        {
            var projects = await _repository.FindAsync(project => project.ProjectStatus.Equals(status));
            if (projects.IsNullOrEmpty())
            {
                _logger.LogInformation("No projects found matching the condition.");
                return new StaticPagedList<ProjectDTO>(new List<ProjectDTO>(), pageNumber, pageSize, 0);
            }
            return projects
                .Select(project => _projectMapper.ToDTO(project))
                .OrderBy(project => project.Name)
                .ToPagedList();
        }
        public async Task<bool> CheckProjectExsitsById(Guid id)
        {
            var project = await _repository.GetByIdAsync(id);
            if (project == null)
            {
                _logger.LogWarning($"Project with ID {id} not found.");
                return false;
            }
            return true;
        }
        public async Task<ProjectDTO> ChangeProjectStatus(Guid id, ProjectStatus newStatus)
        {
            var existingProject = await _repository.GetByIdAsync(id);
            if (existingProject == null)
            {
                _logger.LogWarning($"Project with ID {id} not found.");
                throw new KeyNotFoundException($"Project with ID {id} not found.");
            }
            existingProject.ProjectStatus = newStatus;
            _repository.Update(existingProject);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to change project status.");
                throw new Exception("Failed to change project status.");
            }
            return _projectMapper.ToDTO(existingProject);
        }
    }
}
