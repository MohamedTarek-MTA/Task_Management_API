namespace Task_Management_API.Application.DTOs.Auth
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
