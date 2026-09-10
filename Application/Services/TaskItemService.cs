using Microsoft.IdentityModel.Tokens;
using Task_Management_API.Application.DTOs.TaskItemDTOs;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Application.Mappers;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Domain.Enums;
using Task_Management_API.Application.DTOs.TaskHistoryDTOs;
using Task_Management_API.Infrastructure.Repositories;
using X.PagedList;
using X.PagedList.Extensions;

namespace Task_Management_API.Application.Services
{
    public class TaskItemService : ITaskItemService
    {
        private readonly IRepository<TaskItem> _repository;
        private readonly ILogger<TaskItemService> _logger;
        private readonly TaskItemMapper _taskItemMapper;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly ITaskHistoryService _taskHistoryService;
        private ILogger<TaskItemService> object1;
        private IRepository<TaskItem> object2;

        public TaskItemService(IRepository<TaskItem> repository, ILogger<TaskItemService> logger, TaskItemMapper taskItemMapper, IProjectService projectService, IUserService userService, ITaskHistoryService taskHistoryService)
        {
            _repository = repository;
            _logger = logger;
            _taskItemMapper = taskItemMapper;
            _projectService = projectService;
            _userService = userService;
            _taskHistoryService = taskHistoryService;
        }

        public TaskItemService(ILogger<TaskItemService> object1, IRepository<TaskItem> object2, TaskItemMapper taskItemMapper)
        {
            this.object1 = object1;
            this.object2 = object2;
            _taskItemMapper = taskItemMapper;
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

        public async Task<IPagedList<TaskItemDTO>> GetAllTaskItems(int pageNumber, int pageSize)
        {
            var taskItems =  _repository.GetQueryable()
                .OrderByDescending(ti => ti.CreatedAt)
                .ToPagedList(pageNumber,pageSize);
            if (taskItems.IsNullOrEmpty())
            {
                _logger.LogInformation("No task items found.");
                return new StaticPagedList<TaskItemDTO>(new List<TaskItemDTO>(),pageNumber,pageSize,0);
            }
            var taskItemDTOs = taskItems
                .Select(taskItem => _taskItemMapper.ToDTO(taskItem))
                .ToList();
            return new StaticPagedList<TaskItemDTO>(
                taskItemDTOs,
                pageNumber,
                pageSize,
                taskItems.TotalItemCount);
        }
        public async Task<IPagedList<TaskItemDTO>> GetAllTaskItemsByProjectId(Guid projectId, int pageNumber, int pageSize)
        {
            var taskItems = _repository.GetQueryable()
                .Where(ti => ti.ProjectId == projectId)
                .OrderByDescending(ti => ti.CreatedAt)
                .ToPagedList(pageNumber, pageSize);

            if (taskItems.IsNullOrEmpty())
            {
                _logger.LogInformation($"No task items found for project ID {projectId}.");
                return new StaticPagedList<TaskItemDTO>(new List<TaskItemDTO>(), pageNumber, pageSize, 0);
            }
            var taskItemDTOs = taskItems
                .Select(taskItem => _taskItemMapper.ToDTO(taskItem))
                .ToList();
            return new StaticPagedList<TaskItemDTO>(
                taskItemDTOs,
                pageNumber,
                pageSize,
                taskItems.TotalItemCount);

        }
        public async Task<IPagedList<TaskItemDTO>> GetAllTaskItemsByAssignedUserId(Guid userId, int pageNumber, int pageSize)
        {
            var taskItems = _repository.GetQueryable()
                .Where(ti => ti.AssignedUserId == userId)
                .OrderByDescending(ti => ti.CreatedAt)
                .ToPagedList(pageNumber, pageSize);
            if (taskItems.IsNullOrEmpty())
            {
                _logger.LogInformation($"No task items found for assigned user ID {userId}.");
                return new StaticPagedList<TaskItemDTO>(new List<TaskItemDTO>(), pageNumber, pageSize, 0);
            }
            var taskItemDTOs = taskItems
                .Select(taskItem => _taskItemMapper.ToDTO(taskItem))
                .ToList();
            return new StaticPagedList<TaskItemDTO>(
                taskItemDTOs,
                pageNumber,
                pageSize,
                taskItems.TotalItemCount);
        }
        public async Task<TaskItemDTO> CreateTaskItem(CreateTaskItemDTO taskItemDTO)
        {
            var project = await _projectService.GetProjectById(taskItemDTO.ProjectId);
            if (taskItemDTO.ProjectId == Guid.Empty 
                || project == null 
                || project.ProjectStatus == ProjectStatus.COMPLETED 
                || project.ProjectStatus == ProjectStatus.CANCELLED)
            {
                throw new ArgumentException("Task Needs To Be Assigned To A Valid Project.");
            }
            if(taskItemDTO.DueDate < DateTime.Now)
            {
                throw new ArgumentException("DueDate cannot be earlier than the current date.");
            }
            var taskItem = _taskItemMapper.ToEntity(taskItemDTO);
            await _repository.AddAsync(taskItem);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to create task item.");
                throw new Exception("Failed to create task item.");
            }
            var taskHistoryDTO = new TaskHistoryDTO
            {
                TaskItemId = taskItem.Id,
                Action = "Task Created",
                CreatedAt = DateTime.Now,
                OldValue = null,
                NewValue = $"Task '{taskItem.Title}' created with status '{taskItem.Status}' and priority '{taskItem.Priority}'."
            };
            await _taskHistoryService.CreateTaskHistory(taskHistoryDTO);
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
            if (! await _userService.CheckUserExsitsById(userId))
            {
                throw new ArgumentException("Task Needs To Be Assigned To A Valid User.");
            }
            var updatedTaskItem = _taskItemMapper.ToUpdateDTO(taskItem);
            updatedTaskItem.AssignedUserId = userId;
            _taskItemMapper.Map(updatedTaskItem,taskItem);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to assign task to user.");
                throw new Exception("Failed to assign task to user.");
            }
            var taskHistoryDTO = new TaskHistoryDTO 
            {
                TaskItemId = taskItem.Id,
                Action = "Task Assigned",
                CreatedAt = DateTime.Now,
                OldValue = $"Task '{taskItem.Title}' created with status '{taskItem.Status}' and priority '{taskItem.Priority}'.",
                NewValue = $"Task '{taskItem.Title}' assigned to user with ID {userId}."
            };
            await _taskHistoryService.CreateTaskHistory(taskHistoryDTO);
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
            if(existingTaskItem.Status == TaskItemStatus.COMPLETED || existingTaskItem.Status == TaskItemStatus.CANCELLED)
            {
                throw new InvalidOperationException($"Couldn't change status because it's already {existingTaskItem.Status}");
            }
            var oldStatus = existingTaskItem.Status;
            var updatedTaskItem = _taskItemMapper.ToUpdateDTO(existingTaskItem);
            updatedTaskItem.Status = taskItemStatus;
            if(updatedTaskItem.Status == TaskItemStatus.COMPLETED)
            {
                updatedTaskItem.CompletedAt = DateTime.Now;
            }
            _taskItemMapper.Map(updatedTaskItem, existingTaskItem);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to update task item status.");
                throw new Exception("Failed to update task item status.");
            }
            var taskHistoryDTO = new TaskHistoryDTO
            {
                TaskItemId = existingTaskItem.Id,
                Action = "Task Status Updated",
                CreatedAt = DateTime.Now,
                OldValue = $"Task '{existingTaskItem.Title}' had status '{oldStatus}' and priority '{existingTaskItem.Priority}'.",
                NewValue = $"Task '{existingTaskItem.Title}' updated to status '{taskItemStatus}' and priority '{existingTaskItem.Priority}'."
            };
            await _taskHistoryService.CreateTaskHistory(taskHistoryDTO);
            return _taskItemMapper.ToDTO(existingTaskItem);
        }
        public async Task<TaskItemDTO> UpdateTaskItem(Guid id, UpdateTaskItemDTO taskItemDTO)
        {
            var existingTaskItem = await _repository.GetByIdAsync(id);
            if (existingTaskItem == null)
            {
                _logger.LogWarning($"TaskItem with ID {id} not found.");
                throw new KeyNotFoundException($"TaskItem with ID {id} not found.");
            }
            var oldTaskItem = existingTaskItem;
            _taskItemMapper.Map(taskItemDTO, existingTaskItem);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to update task item.");
                throw new Exception("Failed to update task item.");
            }
            var taskHistoryDTO = new TaskHistoryDTO
            {
                TaskItemId = existingTaskItem.Id,
                Action = "Task Updated",
                CreatedAt = DateTime.Now,
                OldValue = $"Task '{oldTaskItem.Title}' had status '{oldTaskItem.Status}' and priority '{oldTaskItem.Priority}' and assigned to user with ID {oldTaskItem.AssignedUserId}.",
                NewValue = $"Task '{taskItemDTO.Title}' updated to status '{taskItemDTO.Status}' and priority '{taskItemDTO.Priority}'."
            };
            await _taskHistoryService.CreateTaskHistory(taskHistoryDTO);

            return _taskItemMapper.ToDTO(existingTaskItem);
        }
        public async Task DeleteTaskItem(Guid id) 
        {
            var taskItem = await _repository.GetByIdAsync(id);
            if (taskItem == null)
            {
                _logger.LogWarning($"TaskItem with ID {id} not found.");
                throw new KeyNotFoundException($"TaskItem with ID {id} not found.");
            }
            _repository.Remove(taskItem);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to delete task item.");
                throw new Exception("Failed to delete task item.");
            }
        }

        public async Task<bool> CheckTaskItemExsitsById(Guid id)
        {
            return await _repository.AnyAsync(task => task.Id == id);
        }
    }
}
