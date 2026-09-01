using Task_Management_API.Application.DTOs;
using X.PagedList;

namespace Task_Management_API.Application.Interfaces
{
    public interface ITaskHistoryService
    {
        public Task<IPagedList<TaskHistoryDTO>> GetAllTaskItemHistory(Guid id, int pageNumber, int pageSize);
        public Task<TaskHistoryDTO> CreateTaskHistory(TaskHistoryDTO taskHistoryDTO);

    }
}
