using Microsoft.IdentityModel.Tokens;
using Task_Management_API.Application.DTOs.UserDTOs;
using Task_Management_API.Application.Exceptions;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Application.Mappers;
using Task_Management_API.Domain.Entities;
using Task_Management_API.Infrastructure.Repositories;
using X.PagedList;
using X.PagedList.EF;
using X.PagedList.Extensions;

namespace Task_Management_API.Application.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IRepository<User> _repository;
        private readonly UserMapper _userMapper;

        public UserService(ILogger<UserService> logger, IRepository<User> repository, UserMapper userMapper)
        {
            _logger = logger;
            _repository = repository;
            _userMapper = userMapper;
        }

        public async Task<UserDTO> GetUserById(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }
            return _userMapper.ToDTO(user);
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsers()
        {
            var users = await _repository.GetAllAsync();
            if (users.IsNullOrEmpty())
            {
                _logger.LogInformation("No users found.");
                return [];
            }
            return users
                .Select(user => _userMapper.ToDTO(user))
                .OrderBy(user => user.FullName);
        }

        public async Task<IPagedList<UserDTO>> GetAllUsersPaged(int pageNumber, int pageSize)
        {
            var users = await _repository
                .GetQueryable()
                .OrderBy(u=>u.FullName)
                .ToPagedListAsync(pageNumber, pageSize);

            if (users.IsNullOrEmpty())
            {
                _logger.LogInformation("No users found.");
                return new StaticPagedList<UserDTO>(new List<UserDTO>(), pageNumber, pageSize, 0);
            }

            var usersDto = users
                .Select(user => _userMapper.ToDTO(user))
                .ToList();

            return new StaticPagedList<UserDTO>(
                    usersDto,
                    pageNumber,
                    pageSize,
                    users.TotalItemCount);
        }
        public async Task<IPagedList<UserDTO>> GetAllUsersByName(string name, int pageNumber, int pageSize)
        {
            var users = await _repository
                .GetQueryable()
                .Where(u => u.FullName.Contains(name))
                .ToPagedListAsync(pageNumber, pageSize);

            if (users.IsNullOrEmpty())
            {
                _logger.LogInformation("No users found matching the condition.");
                return new StaticPagedList<UserDTO>(new List<UserDTO>(), pageNumber, pageSize, 0);
            }
            var usersDto = users
                .Select(user => _userMapper.ToDTO(user))
                .ToList();

            return new StaticPagedList<UserDTO>(
                    usersDto,
                    pageNumber,
                    pageSize,
                    users.TotalItemCount);
        }
        public async Task<UserDTO> FindUserByEmail(string email)
        {
            var users = await _repository.FindAsync(user => user.Email.Equals(email));
            var user = users.FirstOrDefault();
            if (user == null)
            {
                _logger.LogInformation($"No user found with email: {email}");
                throw new KeyNotFoundException($"User with email {email} not found.");
            }
            return _userMapper.ToDTO(user);
        }

        public async Task<UserDTO> CreateUser(UserDTO userDto)
        {
            if (await _repository.AnyAsync(u=>u.Email == userDto.Email))
            {
                _logger.LogError($"User with email {userDto.Email} already exists.");
                throw new DuplicateResourceException($"User with email {userDto.Email} already exists.");
            }
            var user = _userMapper.ToEntity(userDto);
            await _repository.AddAsync(user);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to create user.");
                throw new Exception("Failed to create user.");
            }
            return _userMapper.ToDTO(user);
        }

        public async Task<UserDTO> UpdateUser(Guid id, UserDTO userDto)
        {
            if (await _repository.AnyAsync(u => u.Email == userDto.Email))
            {
                _logger.LogError($"User with email {userDto.Email} already exists.");
                throw new DuplicateResourceException($"User with email {userDto.Email} already exists.");
            }
            var updatedUser = _userMapper.ToEntity(userDto);
            updatedUser.Id = id;

            _repository.Update(updatedUser);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to update user.");
                throw new Exception("Failed to update user.");
            }
            return _userMapper.ToDTO(updatedUser);
        }
        public async Task DeleteUser(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found.");
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }
            _repository.Remove(user);
            var success = await _repository.SaveChangesAsync();
            if (!success)
            {
                _logger.LogError("Failed to delete user.");
                throw new Exception("Failed to delete user.");
            }
        }
        public async Task<IPagedList<UserDTO>> GetAllUsersByRole(string role, int pageNumber, int pageSize)
        {
            var users = await _repository
                .GetQueryable()
                .Where(u => u.Role == role)
                .ToPagedListAsync(pageNumber, pageSize);

            if (users.IsNullOrEmpty())
            {
                _logger.LogInformation($"No users found with role: {role}");
                return new StaticPagedList<UserDTO>(new List<UserDTO>(), pageNumber, pageSize, 0);
            }
            var usersDto = users
                .Select(user => _userMapper.ToDTO(user))
                .ToList();

            return new StaticPagedList<UserDTO>(
                    usersDto,
                    pageNumber,
                    pageSize,
                    users.TotalItemCount);
        }
        public async Task<bool> CheckUserExsitsById(Guid id)
        {
            if (await _repository.AnyAsync(user => user.Id == id))
            {
                _logger.LogInformation($"User with ID {id} does not exist.");
                return false;
            }
            return true;
        }
    }
}
