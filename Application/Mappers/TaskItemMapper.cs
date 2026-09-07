using Riok.Mapperly.Abstractions;
using Task_Management_API.Application.DTOs.ProjectDTOs;
using Task_Management_API.Application.DTOs.TaskItemDTOs;
using Task_Management_API.Domain.Entities;

namespace Task_Management_API.Application.Mappers
{
    [Mapper]
    public partial class TaskItemMapper
    {
        public partial TaskItemDTO ToDTO(TaskItem taskItem);
        public partial TaskItem ToEntity(TaskItemDTO taskItemDTO);
        public partial void Map(UpdateTaskItemDTO taskItemDTO, [MappingTarget] TaskItem taskItem);
        public partial CreateTaskItemDTO ToCreationDTO(TaskItem taskItem);
        public partial TaskItem ToEntity(CreateTaskItemDTO createTaskItemDTO);

        public partial UpdateTaskItemDTO ToUpdateDTO(TaskItem taskItem);

    }
}
