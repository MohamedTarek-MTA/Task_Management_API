using Task_Management_API.Application.DTOs;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Application.Mappers;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Infrastructure.Repositories;
using X.PagedList;
using X.PagedList.Extensions;

namespace Task_Management_API.Application.Services
{
    public class TaskHistoryService : ITaskHistoryService
    {
        private readonly IRepository<TaskHistory> _repository;
        private readonly ILogger<TaskHistoryService> _logger;
        private readonly TaskHistoryMapper _taskHistoryMapper;
        private readonly IRepository<TaskItem> _taskItemRepository;
        public TaskHistoryService(IRepository<TaskHistory> repository, ILogger<TaskHistoryService> logger, TaskHistoryMapper taskHistoryMapper, IRepository<TaskItem> taskItemRepository)
        {
            _repository = repository;
            _logger = logger;
            _taskHistoryMapper = taskHistoryMapper;
            _taskItemRepository = taskItemRepository;
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
            if (await _taskItemRepository.GetByIdAsync(id) != null)
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