using Microsoft.Extensions.Logging;
using NSubstitute;
using Task_Management_API.Application.DTOs.ProjectDTOs;
using Task_Management_API.Application.DTOs.TaskHistoryDTOs;
using Task_Management_API.Application.DTOs.TaskItemDTOs;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Application.Mappers;
using Task_Management_API.Application.Services;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Domain.Enums;
using Task_Management_API.Tests.Helpers;
using Xunit;

namespace Task_Management_API.Tests.ApplicationTests.ServiceTests;

public class TaskItemServiceTests
{
    private readonly IRepository<TaskItem> _repository = Substitute.For<IRepository<TaskItem>>();
    private readonly ILogger<TaskItemService> _logger = Substitute.For<ILogger<TaskItemService>>();
    private readonly TaskItemMapper _mapper = new();
    private readonly IProjectService _projectService = Substitute.For<IProjectService>();
    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly ITaskHistoryService _taskHistoryService = Substitute.For<ITaskHistoryService>();
    private readonly TaskItemService _sut;

    public TaskItemServiceTests() =>
        _sut = new TaskItemService(
            _repository, _logger, _mapper,
            _projectService, _userService, _taskHistoryService);

    [Fact]
    public async Task CreateTaskItem_WhenProjectIsCompleted_ThrowsArgumentException()
    {
        var projectId = Guid.NewGuid();
        _projectService.GetProjectById(projectId).Returns(new ProjectDTO
        {
            Id = projectId,
            Name = "Done",
            Description = "Done",
            StartDate = DateTime.UtcNow,
            ProjectStatus = ProjectStatus.COMPLETED
        });

        var dto = new CreateTaskItemDTO
        {
            Title = "Task",
            Description = "Desc",
            Priority = TaskPriority.HIGH,
            DueDate = DateTime.UtcNow.AddDays(1),
            ProjectId = projectId
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateTaskItem(dto));
    }

    [Fact]
    public async Task CreateTaskItem_WhenDueDateInPast_ThrowsArgumentException()
    {
        var projectId = Guid.NewGuid();
        _projectService.GetProjectById(projectId).Returns(new ProjectDTO
        {
            Id = projectId,
            Name = "Active",
            Description = "Active",
            StartDate = DateTime.UtcNow,
            ProjectStatus = ProjectStatus.IN_PROGRESS
        });

        var dto = new CreateTaskItemDTO
        {
            Title = "Task",
            Description = "Desc",
            Priority = TaskPriority.HIGH,
            DueDate = DateTime.UtcNow.AddDays(-1),
            ProjectId = projectId
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateTaskItem(dto));
    }

    [Fact]
    public async Task CreateTaskItem_WhenValid_CreatesTaskAndHistory()
    {
        var projectId = Guid.NewGuid();
        _projectService.GetProjectById(projectId).Returns(new ProjectDTO
        {
            Id = projectId,
            Name = "Active",
            Description = "Active",
            StartDate = DateTime.UtcNow,
            ProjectStatus = ProjectStatus.IN_PROGRESS
        });
        _repository.SaveChangesAsync().Returns(true);
        _taskHistoryService.CreateTaskHistory(Arg.Any<TaskHistoryDTO>())
            .Returns(callInfo => callInfo.Arg<TaskHistoryDTO>());

        var dto = new CreateTaskItemDTO
        {
            Title = "Task",
            Description = "Desc",
            Priority = TaskPriority.HIGH,
            DueDate = DateTime.UtcNow.AddDays(1),
            ProjectId = projectId
        };

        var result = await _sut.CreateTaskItem(dto);

        await _repository.Received(1).AddAsync(Arg.Any<TaskItem>());
        await _taskHistoryService.Received(1).CreateTaskHistory(
            Arg.Is<TaskHistoryDTO>(h => h.Action == "Task Created"));
        Assert.Equal("Task", result.Title);
    }

    [Fact]
    public async Task AssigneTaskToUser_WhenUserDoesNotExist_ThrowsArgumentException()
    {
        var task = TestData.CreateTaskItem(Guid.NewGuid());
        _repository.GetByIdAsync(task.Id).Returns(task);
        _userService.CheckUserExsitsById(Arg.Any<Guid>()).Returns(false);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.AssigneTaskToUser(task.Id, Guid.NewGuid()));
    }

    [Fact]
    public async Task AssigneTaskToUser_WhenValid_AssignsUserAndCreatesHistory()
    {
        var userId = Guid.NewGuid();
        var task = TestData.CreateTaskItem(Guid.NewGuid());
        _repository.GetByIdAsync(task.Id).Returns(task);
        _userService.CheckUserExsitsById(userId).Returns(true);
        _repository.SaveChangesAsync().Returns(true);
        _taskHistoryService.CreateTaskHistory(Arg.Any<TaskHistoryDTO>())
            .Returns(callInfo => callInfo.Arg<TaskHistoryDTO>());

        var result = await _sut.AssigneTaskToUser(task.Id, userId);

        Assert.Equal(userId, result.AssignedUserId);
    }

    [Fact]
    public async Task UpdateTaskItemStatus_WhenAlreadyCompleted_ThrowsInvalidOperationException()
    {
        var task = TestData.CreateTaskItem(Guid.NewGuid(), TaskItemStatus.COMPLETED);
        _repository.GetByIdAsync(task.Id).Returns(task);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _sut.UpdateTaskItemStatus(task.Id, TaskItemStatus.IN_PROGRESS));
    }

    [Fact]
    public async Task UpdateTaskItemStatus_WhenSetToCompleted_SetsCompletedAt()
    {
        var task = TestData.CreateTaskItem(Guid.NewGuid(), TaskItemStatus.IN_PROGRESS);
        _repository.GetByIdAsync(task.Id).Returns(task);
        _repository.SaveChangesAsync().Returns(true);
        _taskHistoryService.CreateTaskHistory(Arg.Any<TaskHistoryDTO>())
            .Returns(callInfo => callInfo.Arg<TaskHistoryDTO>());

        var result = await _sut.UpdateTaskItemStatus(task.Id, TaskItemStatus.COMPLETED);

        Assert.Equal(TaskItemStatus.COMPLETED, result.Status);
        Assert.NotNull(result.CompletedAt);
    }

    [Fact]
    public async Task GetAllTaskItemsByProjectId_FiltersByProject()
    {
        var projectId = Guid.NewGuid();
        var tasks = new[]
        {
            TestData.CreateTaskItem(projectId),
            TestData.CreateTaskItem(Guid.NewGuid())
        };
        TestData.SetupQueryable(_repository, tasks);

        var result = await _sut.GetAllTaskItemsByProjectId(projectId, 1, 10);

        Assert.Single(result);
        Assert.Equal(projectId, result.First().ProjectId);
    }
}