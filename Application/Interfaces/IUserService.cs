using Task_Management_API.Application.DTOs.UserDTOs;
using X.PagedList;

namespace Task_Management_API.Application.Interfaces
{
    public interface IUserService
    {
        public Task<UserDTO> GetUserById(Guid id);
        public Task<IEnumerable<UserDTO>> GetAllUsers();
        public Task<IPagedList<UserDTO>> GetAllUsersPaged(int pageNumber, int pageSize);
        public Task<IPagedList<UserDTO>> GetAllUsersByName(string name, int pageNumber, int pageSize);
        public Task<UserDTO> FindUserByEmail(string email);
        public Task<UserDTO> CreateUser(UserDTO userDto);
        public Task<UserDTO> UpdateUser(Guid id, UserDTO userDto);
        public Task DeleteUser(Guid id);
        public Task<IPagedList<UserDTO>> GetAllUsersByRole(string role, int pageNumber, int pageSize);
        public Task<bool> CheckUserExsitsById(Guid id);

    }
}
