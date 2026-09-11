using Microsoft.Extensions.Logging;
using NSubstitute;
using Task_Management_API.Application.DTOs.ProjectDTOs;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Application.Mappers;
using Task_Management_API.Application.Services;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Domain.Enums;
using Task_Management_API.Tests.Helpers;

namespace Task_Management_API.Tests.ApplicationTests.ServiceTests;

public class ProjectServiceTests
{
    private readonly ILogger<ProjectService> _logger = Substitute.For<ILogger<ProjectService>>();
    private readonly IRepository<Project> _repository = Substitute.For<IRepository<Project>>();
    private readonly IRepository<TaskItem> _taskRepository = Substitute.For<IRepository<TaskItem>>();
    private readonly ProjectMapper _mapper = new();
    private readonly ProjectService _sut;

    public ProjectServiceTests() =>
        _sut = new ProjectService(_logger, _repository, _taskRepository, _mapper);

    [Fact]
    public async Task GetProjectById_WhenFound_ReturnsDto()
    {
        var project = TestData.CreateProject();
        _repository.GetByIdAsync(project.Id).Returns(project);

        var result = await _sut.GetProjectById(project.Id);

        Assert.Equal(project.Name, result.Name);
    }

    [Fact]
    public async Task GetProjectById_WhenNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id).Returns((Project?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetProjectById(id));
    }

    [Fact]
    public async Task CreateProject_WhenValid_CreatesProject()
    {
        var dto = new CreateProjectDTO
        {
            Name = "New Project",
            Description = "Desc",
            StartDate = DateTime.UtcNow,
            ProjectStatus = ProjectStatus.TODO
        };
        _repository.SaveChangesAsync().Returns(true);

        var result = await _sut.CreateProject(dto);

        await _repository.Received(1).AddAsync(Arg.Any<Project>());
        Assert.Equal("New Project", result.Name);
    }

    [Fact]
    public async Task UpdateProject_WhenStartDateAfterEndDate_ThrowsArgumentException()
    {
        var project = TestData.CreateProject();
        project.StartDate = DateTime.UtcNow.AddDays(10);
        project.EndDate = DateTime.UtcNow;
        _repository.GetByIdAsync(project.Id).Returns(project);

        var updateDto = new UpdateProjectDTO
        {
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.UpdateProject(project.Id, updateDto));
    }

    [Fact]
    public async Task DeleteProject_WhenActiveTasksExist_ThrowsInvalidOperationException()
    {
        var project = TestData.CreateProject();
        _repository.GetByIdAsync(project.Id).Returns(project);
        _taskRepository.AnyAsync(Arg.Any<System.Linq.Expressions.Expression<Func<TaskItem, bool>>>())
            .Returns(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.DeleteProject(project.Id));
    }

    [Fact]
    public async Task DeleteProject_WhenNoActiveTasks_DeletesProject()
    {
        var project = TestData.CreateProject();
        _repository.GetByIdAsync(project.Id).Returns(project);
        _taskRepository.AnyAsync(Arg.Any<System.Linq.Expressions.Expression<Func<TaskItem, bool>>>())
            .Returns(false);
        _repository.SaveChangesAsync().Returns(true);

        await _sut.DeleteProject(project.Id);

        _repository.Received(1).Remove(project);
    }

    [Fact]
    public async Task ChangeProjectStatus_WhenFound_UpdatesStatus()
    {
        var project = TestData.CreateProject(status: ProjectStatus.TODO);
        _repository.GetByIdAsync(project.Id).Returns(project);
        _repository.SaveChangesAsync().Returns(true);

        var result = await _sut.ChangeProjectStatus(project.Id, ProjectStatus.COMPLETED);

        Assert.Equal(ProjectStatus.COMPLETED, result.ProjectStatus);
        _repository.Received(1).Update(project);
    }

    [Fact]
    public async Task FindProjectsByStatus_FiltersByStatus()
    {
        var projects = new[]
        {
            TestData.CreateProject("A", ProjectStatus.IN_PROGRESS),
            TestData.CreateProject("B", ProjectStatus.COMPLETED)
        };
        TestData.SetupQueryable(_repository, projects);

        var result = await _sut.FindProjectsByStatus(ProjectStatus.IN_PROGRESS, 1, 10);

        Assert.Single(result);
        Assert.Equal(ProjectStatus.IN_PROGRESS, result.First().ProjectStatus);
    }
}