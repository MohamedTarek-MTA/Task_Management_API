using Microsoft.AspNetCore.Identity;
using Task_Management_API.Application.Interfaces;

namespace Task_Management_API.Application.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<object> _passwordHasher= new ();
        public string HashPassword(string password)
        {
            return _passwordHasher.HashPassword(null!, password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, password);
            return result == PasswordVerificationResult.Success 
                || result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
