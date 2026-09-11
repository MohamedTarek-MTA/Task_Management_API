using MockQueryable.NSubstitute;
using NSubstitute;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Domain.Enums;

namespace Task_Management_API.Tests.Helpers;

public static class TestData
{
    public static User CreateUser(
        string fullName = "Alice Smith",
        string email = "alice@test.com",
        string role = "Developer")
        => new()
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

    public static Project CreateProject(
        string name = "Project Alpha",
        ProjectStatus status = ProjectStatus.IN_PROGRESS)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = "Test project",
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30),
            ProjectStatus = status
        };

    public static TaskItem CreateTaskItem(
        Guid projectId,
        TaskItemStatus status = TaskItemStatus.TODO,
        Guid? assignedUserId = null)
        => new()
        {
            Id = Guid.NewGuid(),
            Title = "Test Task",
            Description = "Test description",
            Status = status,
            Priority = TaskPriority.MEDIUM,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            ProjectId = projectId,
            AssignedUserId = assignedUserId
        };

    public static TaskHistory CreateTaskHistory(Guid taskItemId, string action = "Task Created")
        => new()
        {
            Id = Guid.NewGuid(),
            TaskItemId = taskItemId,
            Action = action,
            OldValue = null,
            NewValue = "Created",
            CreatedAt = DateTime.UtcNow
        };

    public static void SetupQueryable<T>(IRepository<T> repository, IEnumerable<T> data)
        where T : class
    {
        var queryable = data.ToList().BuildMockDbSet();
            repository.GetQueryable().Returns(queryable);
    }
}