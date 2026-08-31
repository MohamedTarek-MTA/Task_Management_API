using Task_Management_API.Application.DTOs;
using Task_Management_API.Application.Mappers;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Infrastructure.Repositories;
using X.PagedList;
using X.PagedList.Extensions;

namespace Task_Management_API.Application.Services
{
    public class TaskHistoryService
    {
        private readonly Repository<TaskHistory> _repository;
        private readonly ILogger<TaskHistoryService> _logger;
        private readonly TaskHistoryMapper _taskHistoryMapper;
        private readonly TaskItemService _taskItemService;
        public TaskHistoryService(Repository<TaskHistory> repository, ILogger<TaskHistoryService> logger, TaskHistoryMapper taskHistoryMapper, TaskItemService taskItemService)
        {
            _repository = repository;
            _logger = logger;
            _taskHistoryMapper = taskHistoryMapper;
            _taskItemService = taskItemService;
        }

        public async Task<TaskHistoryDTO> GetTaskHistoryById(Guid id)
        {
            var taskHistory = await _repository.GetByIdAsync(id);
            if (taskHistory == null)
            {
                _logger.LogWarning($"TaskHistory with ID {id} not found.");
                throw new KeyNotFoundException($"TaskHistory with ID {id} not found.");
            }
            return _taskHistoryMapper.ToDTO(taskHistory);
        }
        public async Task<IPagedList<TaskHistoryDTO>> GetAllTaskItemHistory(Guid id, int pageNumber, int pageSize)
        {
            if (await _taskItemService.CheckTaskItemExsitsById(id))
            {
                var taskHistorys = await _repository.FindAsync(th => th.TaskItemId == id);
                return taskHistorys.
                    Select(th => _taskHistoryMapper.ToDTO(th))
                    .OrderByDescending(th => th.CreatedAt)
                    .ToPagedList(pageNumber, pageSize);
            }
            else
            {
                _logger.LogWarning($"TaskItem with ID {id} not found.");
                throw new KeyNotFoundException($"TaskItem with ID {id} not found.");
            }
        }
        public async Task<TaskHistoryDTO> CreateTaskHistory(TaskHistoryDTO taskHistoryDTO)
        {
            var taskHistory = _taskHistoryMapper.ToEntity(taskHistoryDTO);
            await _repository.AddAsync(taskHistory);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to create task history.");
                throw new Exception("Failed to create task history.");
            }
            return _taskHistoryMapper.ToDTO(taskHistory);
        }


    }
}