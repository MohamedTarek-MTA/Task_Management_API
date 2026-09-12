using Task_Management_API.Application.DTOs.Auth;

namespace Task_Management_API.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    }
}
