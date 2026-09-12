using Task_Management_API.Domain.Entities;

namespace Task_Management_API.Application.Interfaces
{
    public interface IJwtService
    {
        public string GenerateToken(User user);
    }
}
