using Riok.Mapperly.Abstractions;
using Task_Management_API.Application.DTOs;
using Task_Management_API.Domain.Entities;

namespace Task_Management_API.Application.Mappers
{
    [Mapper]
    public partial class TaskHistoryMapper
    {
        public partial TaskHistoryDTO ToDTO(TaskHistory taskHistory);
        public partial TaskHistory ToEntity(TaskHistoryDTO taskHistoryDTO);
    }
}
