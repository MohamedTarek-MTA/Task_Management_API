using Microsoft.IdentityModel.Tokens;
using Task_Management_API.Application.DTOs.ProjectDTOs;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Application.Mappers;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Domain.Enums;
using Task_Management_API.Infrastructure.Repositories;
using X.PagedList;
using X.PagedList.EF;
using X.PagedList.Extensions;

namespace Task_Management_API.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ILogger<ProjectService> _logger;
        private readonly IRepository<Project> _repository;
        private readonly IRepository<TaskItem> _taskItemRepository;
        private readonly ProjectMapper _projectMapper;
        public ProjectService(ILogger<ProjectService> logger, IRepository<Project> repository, IRepository<TaskItem> taskItemRepository, ProjectMapper projectMapper)
        {
            _logger = logger;
            _repository = repository;
            _taskItemRepository = taskItemRepository;
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
            if (projects == null)
            {
                _logger.LogInformation("No projects found.");
                return [];
            }
            return projects
                .Select(project => _projectMapper.ToDTO(project))
                .OrderBy(project => project.Name);
        }
        public async Task<IPagedList<ProjectDTO>> GetAllProjectsPaged(int pageNumber, int pageSize)
        {
            var projects = await _repository
                .GetQueryable()
                .OrderBy(p => p.Name)
                .ToPagedListAsync(pageNumber, pageSize);
            if (projects == null)
            {
                _logger.LogInformation("No projects found.");
                return new PagedList<ProjectDTO>(new List<ProjectDTO>(), pageNumber, pageSize);
            }

            var projectDTOs = projects
                .Select(project => _projectMapper.ToDTO(project))
                .ToList();

            return new StaticPagedList<ProjectDTO>(
                    projectDTOs,
                    pageNumber,
                    pageSize,
                    projects.TotalItemCount);
        }
        public async Task<ProjectDTO> CreateProject(CreateProjectDTO projectDTO)
        {
            var project = _projectMapper.ToEntity(projectDTO);
            await _repository.AddAsync(project);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to create project.");
                throw new Exception("Failed to create project.");
            }
            return _projectMapper.ToDTO(project);
        }
        public async Task<ProjectDTO> UpdateProject(Guid id, UpdateProjectDTO projectDTO)
        {
            var existingProject = await _repository.GetByIdAsync(id);
            if (existingProject == null)
            {
                _logger.LogWarning($"Project with ID {id} not found.");
                throw new KeyNotFoundException($"Project with ID {id} not found.");
            }
            var startDate = projectDTO.StartDate ?? existingProject.StartDate;
            var endDate = projectDTO.EndDate ?? existingProject.EndDate;

            if (startDate > endDate)
            {
                throw new ArgumentException(
                    "Start date cannot be later than end date.");
            }
            _projectMapper.Map(projectDTO, existingProject);

            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to update project.");
                throw new Exception("Failed to update project.");
            }

            return _projectMapper.ToDTO(existingProject);
        }
        public async Task DeleteProject(Guid id)
        {
            var existingProject = await _repository.GetByIdAsync(id);

            if (existingProject == null)
            {
                _logger.LogWarning(
                    "Project with ID {ProjectId} not found.",
                    id);

                throw new KeyNotFoundException(
                    $"Project with ID {id} not found.");
            }

            var hasActiveTasks = await _taskItemRepository.AnyAsync(
                task => task.ProjectId == id &&
                        task.Status != TaskItemStatus.COMPLETED &&
                        task.Status != TaskItemStatus.CANCELLED);

            if (hasActiveTasks)
            {
                _logger.LogWarning(
                    "Cannot delete project {ProjectId} because it contains active tasks.",
                    id);

                throw new InvalidOperationException(
                    "Cannot delete the project because it contains active tasks.");
            }

            _repository.Remove(existingProject);

            var success = await _repository.SaveChangesAsync();

            if (!success)
            {
                _logger.LogError(
                    "Failed to delete project {ProjectId}.",
                    id);

                throw new Exception("Failed to delete project.");
            }
        }
        public async Task<IPagedList<ProjectDTO>> FindProjectsByName(string name, int pageNumber, int pageSize)
        {
            var projects = await _repository
                    .GetQueryable()
                    .OrderBy(p => p.Name)
                    .ToPagedListAsync(pageNumber, pageSize);
            if (projects==null)
            {
                _logger.LogInformation("No projects found matching the condition.");
                return new StaticPagedList<ProjectDTO>(new List<ProjectDTO>(), pageNumber, pageSize, 0);
            }

            var projectDTOs = projects
                .Select(project => _projectMapper.ToDTO(project))
                .ToList();

            return new StaticPagedList<ProjectDTO>(
                    projectDTOs,
                    pageNumber,
                    pageSize,
                    projects.TotalItemCount);
        }

        public async Task<IPagedList<ProjectDTO>> FindProjectsByStatus(ProjectStatus status, int pageNumber, int pageSize)
        {
            var projects = await _repository
                .GetQueryable()
                .Where(p => p.ProjectStatus == status)
                .OrderBy(p => p.Name)
                .ToPagedListAsync(pageNumber, pageSize);
            if (projects == null)
            {
                _logger.LogInformation("No projects found matching the condition.");
                return new StaticPagedList<ProjectDTO>(new List<ProjectDTO>(), pageNumber, pageSize, 0);
            }
            var projectDTOs = projects
                 .Select(project => _projectMapper.ToDTO(project))
                 .ToList();

            return new StaticPagedList<ProjectDTO>(
                    projectDTOs,
                    pageNumber,
                    pageSize,
                    projects.TotalItemCount);
        }
        public async Task<bool> CheckProjectExsitsById(Guid id)
        {
            return await _repository.AnyAsync(project => project.Id == id);
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
