using Task_Management_API.Application.DTOs.TaskItemDTOs;
using Task_Management_API.Domain.Enums;
using X.PagedList;

namespace Task_Management_API.Application.Interfaces
{
    public interface ITaskItemService
    {
        public Task<TaskItemDTO> GetTaskItemById(Guid id);
        public Task<IEnumerable<TaskItemDTO>> GetAllTaskItems();
        public Task<IPagedList<TaskItemDTO>> GetAllTaskItemsByProjectId(Guid projectId, int pageNumber, int pageSize);
        public Task<IPagedList<TaskItemDTO>> GetAllTaskItemsByAssignedUserId(Guid userId, int pageNumber, int pageSize);
        public Task<TaskItemDTO> CreateTaskItem(TaskItemDTO taskItemDTO);
        public Task<TaskItemDTO> AssigneTaskToUser(Guid taskItemId, Guid userId);
        public Task<TaskItemDTO> UpdateTaskItemStatus(Guid id, TaskItemStatus taskItemStatus);
        public Task<TaskItemDTO> UpdateTaskItem(Guid id, TaskItemDTO taskItemDTO);
        public Task DeleteTaskItem(Guid id);
        public Task<bool> CheckTaskItemExsitsById(Guid id);
    }
}
