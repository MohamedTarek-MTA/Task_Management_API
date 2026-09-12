using Task_Management_API.Application.DTOs.Auth;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Domain.Entities;

namespace Task_Management_API.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _repository;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<AuthService> _logger;
        private readonly IJwtService _jwtService;
        public AuthService(IRepository<User> repository, IPasswordService passwordService, ILogger<AuthService> logger, IJwtService jwtService)
        {
            _repository = repository;
            _passwordService = passwordService;
            _logger = logger;
            _jwtService = jwtService;
        }
        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDto)
        {
            var users = await _repository.FindAsync(u => u.Email == loginRequestDto.Email);
            var user = users.FirstOrDefault();

            if (user == null)
            {
                _logger.LogError("Invalid Email or Password");
                throw new UnauthorizedAccessException("Invalid Email or Password");
            }

            var isPasswordValid = _passwordService.VerifyPassword(loginRequestDto.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogError("Invalid Email or Password");
                throw new UnauthorizedAccessException("Invalid Email or Password");
            }
            var token = _jwtService.GenerateToken(user);
            return new LoginResponseDTO
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15)
            };
        }
    } 
}
