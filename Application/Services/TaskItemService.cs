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
    public class TaskItemService : ITaskItemService
    {
        private readonly Repository<TaskItem> _repository;
        private readonly ILogger<TaskItemService> _logger;
        private readonly TaskItemMapper _taskItemMapper;
        private readonly ProjectService _projectService;
        private readonly UserService _userService;
        public TaskItemService(Repository<TaskItem> repository, ILogger<TaskItemService> logger, TaskItemMapper taskItemMapper, ProjectService projectService, UserService userService)
        {
            _repository = repository;
            _logger = logger;
            _taskItemMapper = taskItemMapper;
            _projectService = projectService;
            _userService = userService;
        }

        public async Task<TaskItemDTO> GetTaskItemById(Guid id)
        {
            var taskItem = await _repository.GetByIdAsync(id);
            if (taskItem == null)
            {
                _logger.LogWarning($"TaskItem with ID {id} not found.");
                throw new KeyNotFoundException($"TaskItem with ID {id} not found.");
            }
            return _taskItemMapper.ToDTO(taskItem);
        }

        public async Task<IEnumerable<TaskItemDTO>> GetAllTaskItems()
        {
            var taskItems = await _repository.GetAllAsync();
            if (taskItems.IsNullOrEmpty())
            {
                _logger.LogInformation("No task items found.");
                return [];
            }
            return taskItems
                .Select(taskItem => _taskItemMapper.ToDTO(taskItem))
                .OrderBy(taskItem => taskItem.CreatedAt);
        }
        public async Task<IPagedList<TaskItemDTO>> GetAllTaskItemsByProjectId(Guid projectId, int pageNumber, int pageSize)
        {
            var taskItems = await _repository.GetAllAsync();
            if (taskItems.IsNullOrEmpty())
            {
                _logger.LogInformation($"No task items found for project ID {projectId}.");
                return new StaticPagedList<TaskItemDTO>(new List<TaskItemDTO>(), pageNumber, pageSize, 0);
            }
            var filteredTaskItems = taskItems.Where(taskItem => taskItem.ProjectId == projectId);
            if (filteredTaskItems.IsNullOrEmpty())
            {
                _logger.LogInformation($"No task items found for project ID {projectId}.");
                return new StaticPagedList<TaskItemDTO>(new List<TaskItemDTO>(), pageNumber, pageSize, 0);
            }
            return filteredTaskItems
                .Select(taskItem => _taskItemMapper.ToDTO(taskItem))
                .OrderBy(taskItem => taskItem.CreatedAt)
                .ToPagedList();
        }
        public async Task<IPagedList<TaskItemDTO>> GetAllTaskItemsByAssignedUserId(Guid userId, int pageNumber, int pageSize)
        {
            var taskItems = await _repository.GetAllAsync();
            if (taskItems.IsNullOrEmpty())
            {
                _logger.LogInformation($"No task items found for assigned user ID {userId}.");
                return new StaticPagedList<TaskItemDTO>(new List<TaskItemDTO>(), pageNumber, pageSize, 0);
            }
            var filteredTaskItems = taskItems.Where(taskItem => taskItem.AssignedUserId == userId);
            if (filteredTaskItems.IsNullOrEmpty())
            {
                _logger.LogInformation($"No task items found for assigned user ID {userId}.");
                return new StaticPagedList<TaskItemDTO>(new List<TaskItemDTO>(), pageNumber, pageSize, 0);
            }
            return filteredTaskItems
                .Select(taskItem => _taskItemMapper.ToDTO(taskItem))
                .OrderBy(taskItem => taskItem.CreatedAt)
                .ToPagedList();
        }
        public async Task<TaskItemDTO> CreateTaskItem(TaskItemDTO taskItemDTO)
        {
            var project = await _projectService.GetProjectById(taskItemDTO.ProjectId);
            if (taskItemDTO.ProjectId == Guid.Empty 
                || project == null 
                || project.ProjectStatus == ProjectStatus.COMPLETED 
                || project.ProjectStatus == ProjectStatus.CANCELLED)
            {
                throw new ArgumentException("Task Needs To Be Assigned To A Valid Project.");
            }
            var taskItem = _taskItemMapper.ToEntity(taskItemDTO);
            await _repository.AddAsync(taskItem);
            return _taskItemMapper.ToDTO(taskItem);
        }
        public async Task<TaskItemDTO> AssigneTaskToUser(Guid taskItemId, Guid userId)
        {
            var taskItem = await _repository.GetByIdAsync(taskItemId);
            if (taskItem == null)
            {
                _logger.LogWarning($"TaskItem with ID {taskItemId} not found.");
                throw new KeyNotFoundException($"TaskItem with ID {taskItemId} not found.");
            }
            var user = await _userService.CheckUserExsitsById(userId);
            if (!user)
            {
                throw new ArgumentException("Task Needs To Be Assigned To A Valid User.");
            }
            taskItem.AssignedUserId = userId;
            _repository.Update(taskItem);
            return _taskItemMapper.ToDTO(taskItem);
        }
        public async Task<TaskItemDTO> UpdateTaskItemStatus(Guid id, TaskItemStatus taskItemStatus)
        {
            var existingTaskItem = await _repository.GetByIdAsync(id);
            if (existingTaskItem == null)
            {
                _logger.LogWarning($"TaskItem with ID {id} not found.");
                throw new KeyNotFoundException($"TaskItem with ID {id} not found.");
            }
            existingTaskItem.Status = taskItemStatus;
            _repository.Update(existingTaskItem);
            return _taskItemMapper.ToDTO(existingTaskItem);
        }
        public async Task<TaskItemDTO> UpdateTaskItem(Guid id, TaskItemDTO taskItemDTO)
        {
            var existingTaskItem = await _repository.GetByIdAsync(id);
            if (existingTaskItem == null)
            {
                _logger.LogWarning($"TaskItem with ID {id} not found.");
                throw new KeyNotFoundException($"TaskItem with ID {id} not found.");
            }
            var updatedTaskItem = _taskItemMapper.ToEntity(taskItemDTO);
            updatedTaskItem.Id = existingTaskItem.Id; 
            _repository.Update(updatedTaskItem);
            return _taskItemMapper.ToDTO(updatedTaskItem);
        }
        public async Task DeleteTaskItemStatus(Guid id) 
        {
            var taskItem = await _repository.GetByIdAsync(id);
            if (taskItem == null)
            {
                _logger.LogWarning($"TaskItem with ID {id} not found.");
                throw new KeyNotFoundException($"TaskItem with ID {id} not found.");
            }
            _repository.Remove(taskItem);
        }
    }
}
