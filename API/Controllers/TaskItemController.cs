using Microsoft.AspNetCore.Mvc;
using Task_Management_API.Application.DTOs.TaskItemDTOs;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Domain.Enums;

namespace Task_Management_API.API.Controllers
{
    [ApiController]
    [Route("api/taskitems")]
    public class TaskItemController : ControllerBase
    {
        private readonly ITaskItemService _taskItemService;
        private readonly ITaskHistoryService _taskHistoryService;
        public TaskItemController(ITaskItemService taskItemService, ITaskHistoryService taskHistoryService)
        {
            _taskItemService = taskItemService;
            _taskHistoryService = taskHistoryService;
        }

        [HttpGet]
        public IActionResult GetAllTaskItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var taskItems = _taskItemService.GetAllTaskItems(pageNumber, pageSize);
            return Ok(taskItems);
        }
        [HttpGet("{id}")]
        public IActionResult GetTaskItemById(Guid id)
        {
            var taskItem = _taskItemService.GetTaskItemById(id);
            if (taskItem == null)
            {
                return NotFound();
            }
            return Ok(taskItem);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTaskItem([FromBody] CreateTaskItemDTO taskItemDTO)
        {
            var createdTaskItem = await _taskItemService.CreateTaskItem(taskItemDTO);
            return CreatedAtAction(nameof(GetTaskItemById), new { id = createdTaskItem.Id }, createdTaskItem);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskItem(Guid id, [FromBody] UpdateTaskItemDTO taskItemDTO)
        {
            var updatedTaskItem = await _taskItemService.UpdateTaskItem(id, taskItemDTO);
            return Ok(updatedTaskItem);
        }
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateTaskItemStatus(Guid id, [FromBody] TaskItemStatus taskItemStatus)
        {
            var updatedTaskItem = await _taskItemService.UpdateTaskItemStatus(id, taskItemStatus);
            return Ok(updatedTaskItem);
        }

        [HttpPatch("{id}/assign/{userId}")]
        public async Task<IActionResult> AssignTaskItem(Guid id, Guid userId)
        {
            var assignedTaskItem = await _taskItemService.AssigneTaskToUser(id, userId);
            return Ok(assignedTaskItem);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(Guid id)
        {
            await _taskItemService.DeleteTaskItem(id);
            return NoContent();
        }
        [HttpGet("{id}/history")]
        public async Task<IActionResult> GetTaskItemHistory(Guid id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var taskItemHistory = await _taskHistoryService.GetAllTaskItemHistory(id, pageNumber, pageSize);
            return Ok(taskItemHistory);
        }
    }
}
