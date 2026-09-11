using Microsoft.Extensions.Logging;
using NSubstitute;
using Task_Management_API.Application.DTOs.UserDTOs;
using Task_Management_API.Application.Exceptions;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Application.Mappers;
using Task_Management_API.Application.Services;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Tests.Helpers;
using Xunit;

namespace Task_Management_API.Tests.ApplicationTests.ServiceTests;

public class UserServiceTests
{
    private readonly ILogger<UserService> _logger = Substitute.For<ILogger<UserService>>();
    private readonly IRepository<User> _repository = Substitute.For<IRepository<User>>();
    private readonly UserMapper _mapper = new();
    private readonly UserService _sut;

    public UserServiceTests() => _sut = new UserService(_logger, _repository, _mapper);

    [Fact]
    public async Task GetUserById_WhenUserExists_ReturnsUserDto()
    {
        var user = TestData.CreateUser();
        _repository.GetByIdAsync(user.Id).Returns(user);

        var result = await _sut.GetUserById(user.Id);

        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.FullName, result.FullName);
    }

    [Fact]
    public async Task GetUserById_WhenUserNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id).Returns((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetUserById(id));
    }

    [Fact]
    public async Task GetAllUsers_WhenUsersExist_ReturnsOrderedByFullName()
    {
        var users = new[] { TestData.CreateUser("Zara"), TestData.CreateUser("Adam") };
        _repository.GetAllAsync().Returns(users);

        var result = (await _sut.GetAllUsers()).ToList();

        Assert.Equal(["Adam", "Zara"], result.Select(u => u.FullName));
    }

    [Fact]
    public async Task GetAllUsers_WhenEmpty_ReturnsEmptyList()
    {
        _repository.GetAllAsync().Returns(Array.Empty<User>());

        var result = await _sut.GetAllUsers();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetUserByEmail_WhenFound_ReturnsUserDto()
    {
        var user = TestData.CreateUser();
        _repository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
            .Returns(new[] { user });

        var result = await _sut.GetUserByEmail(user.Email);

        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetUserByEmail_WhenNotFound_ThrowsKeyNotFoundException()
    {
        _repository.FindAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
            .Returns(Array.Empty<User>());

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _sut.GetUserByEmail("missing@test.com"));
    }

    [Fact]
    public async Task CreateUser_WhenEmailAlreadyExists_ThrowsDuplicateResourceException()
    {
        var dto = new CreateUserDTO
        {
            FullName = "Bob",
            Email = "bob@test.com",
            Role = "Manager"
        };
        _repository.AnyAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
            .Returns(true);

        await Assert.ThrowsAsync<DuplicateResourceException>(() => _sut.CreateUser(dto));
    }

    [Fact]
    public async Task CreateUser_WhenValid_PersistsAndReturnsDto()
    {
        var dto = new CreateUserDTO
        {
            FullName = "Bob",
            Email = "bob@test.com",
            Role = "Manager"
        };
        _repository.AnyAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
            .Returns(false);
        _repository.SaveChangesAsync().Returns(true);

        var result = await _sut.CreateUser(dto);

        await _repository.Received(1).AddAsync(Arg.Any<User>());
        Assert.Equal(dto.Email, result.Email);
    }

    [Fact]
    public async Task CreateUser_WhenSaveFails_ThrowsException()
    {
        var dto = new CreateUserDTO { FullName = "Bob", Email = "bob@test.com", Role = "Manager" };
        _repository.AnyAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>()).Returns(false);
        _repository.SaveChangesAsync().Returns(false);

        await Assert.ThrowsAsync<Exception>(() => _sut.CreateUser(dto));
    }

    [Fact]
    public async Task UpdateUser_WhenUserExists_UpdatesAndReturnsDto()
    {
        var user = TestData.CreateUser();
        var updateDto = new UpdateUserDTO { FullName = "Updated Name" };
        _repository.GetByIdAsync(user.Id).Returns(user);
        _repository.SaveChangesAsync().Returns(true);

        var result = await _sut.UpdateUser(user.Id, updateDto);

        Assert.Equal("Updated Name", result.FullName);
    }

    [Fact]
    public async Task DeleteUser_WhenUserExists_RemovesUser()
    {
        var user = TestData.CreateUser();
        _repository.GetByIdAsync(user.Id).Returns(user);
        _repository.SaveChangesAsync().Returns(true);

        await _sut.DeleteUser(user.Id);

        _repository.Received(1).Remove(user);
    }

    [Fact]
    public async Task DeleteUser_WhenUserNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetByIdAsync(id).Returns((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteUser(id));
    }

    [Fact]
    public async Task CheckUserExsitsById_ReturnsRepositoryResult()
    {
        var id = Guid.NewGuid();
        _repository.AnyAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>()).Returns(true);

        var exists = await _sut.CheckUserExsitsById(id);

        Assert.True(exists);
    }

    [Fact]
    public async Task GetAllUsersPaged_WhenUsersExist_ReturnsPagedDtos()
    {
        var users = Enumerable.Range(1, 5).Select(i => TestData.CreateUser($"User {i}")).ToList();
        TestData.SetupQueryable(_repository, users);

        var result = await _sut.GetAllUsersPaged(1, 2);

        Assert.Equal(2, result.Count);
        Assert.Equal(5, result.TotalItemCount);
    }
}