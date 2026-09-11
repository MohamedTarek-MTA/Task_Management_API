using Microsoft.Extensions.Logging;
using NSubstitute;
using Task_Management_API.Application.DTOs.TaskHistoryDTOs;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Application.Mappers;
using Task_Management_API.Application.Services;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Tests.Helpers;

namespace Task_Management_API.Tests.ApplicationTests.ServiceTests;

public class TaskHistoryServiceTests
{
    private readonly IRepository<TaskHistory> _repository = Substitute.For<IRepository<TaskHistory>>();
    private readonly ILogger<TaskHistoryService> _logger = Substitute.For<ILogger<TaskHistoryService>>();
    private readonly TaskHistoryMapper _mapper = new();
    private readonly IRepository<TaskItem> _taskItemRepository = Substitute.For<IRepository<TaskItem>>();
    private readonly TaskHistoryService _sut;

    public TaskHistoryServiceTests() =>
        _sut = new TaskHistoryService(_repository, _logger, _mapper, _taskItemRepository);

    [Fact]
    public async Task GetTaskHistoryById_WhenFound_ReturnsDto()
    {
        var history = TestData.CreateTaskHistory(Guid.NewGuid());
        _repository.GetByIdAsync(history.Id).Returns(history);

        var result = await _sut.GetTaskHistoryById(history.Id);

        Assert.Equal(history.Action, result.Action);
    }

    [Fact]
    public async Task GetTaskHistoryById_WhenNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id).Returns((TaskHistory?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetTaskHistoryById(id));
    }

    [Fact]
    public async Task GetAllTaskItemHistory_WhenTaskExists_ReturnsPagedHistory()
    {
        var taskId = Guid.NewGuid();
        var histories = new[]
        {
            TestData.CreateTaskHistory(taskId, "Created"),
            TestData.CreateTaskHistory(taskId, "Updated")
        };
        _taskItemRepository.AnyAsync(Arg.Any<System.Linq.Expressions.Expression<Func<TaskItem, bool>>>())
            .Returns(true);
        TestData.SetupQueryable(_repository, histories);

        var result = await _sut.GetAllTaskItemHistory(taskId, 1, 10);

        Assert.Equal(2, result.Count);
        Assert.All(result, h => Assert.Equal(taskId, h.TaskItemId));
    }

    [Fact]
    public async Task GetAllTaskItemHistory_WhenTaskNotFound_ThrowsKeyNotFoundException()
    {
        var taskId = Guid.NewGuid();
        _taskItemRepository.AnyAsync(Arg.Any<System.Linq.Expressions.Expression<Func<TaskItem, bool>>>())
            .Returns(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.GetAllTaskItemHistory(taskId, 1, 10));
    }

    [Fact]
    public async Task CreateTaskHistory_WhenValid_PersistsAndReturnsDto()
    {
        var dto = new TaskHistoryDTO
        {
            TaskItemId = Guid.NewGuid(),
            Action = "Task Created",
            CreatedAt = DateTime.UtcNow,
            NewValue = "Created"
        };
        _repository.SaveChangesAsync().Returns(true);

        var result = await _sut.CreateTaskHistory(dto);

        await _repository.Received(1).AddAsync(Arg.Any<TaskHistory>());
        Assert.Equal("Task Created", result.Action);
    }

    [Fact]
    public async Task CreateTaskHistory_WhenSaveFails_ThrowsException()
    {
        var dto = new TaskHistoryDTO
        {
            TaskItemId = Guid.NewGuid(),
            Action = "Task Created",
            CreatedAt = DateTime.UtcNow
        };
        _repository.SaveChangesAsync().Returns(false);

        await Assert.ThrowsAsync<Exception>(() => _sut.CreateTaskHistory(dto));
    }
}