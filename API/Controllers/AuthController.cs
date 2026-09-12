using Microsoft.AspNetCore.Mvc;
using Task_Management_API.Application.DTOs.Auth;
using Task_Management_API.Application.Interfaces;

namespace Task_Management_API.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDTO)
        {
            var loginResponse = await _authService.Login(loginDTO);
            if (loginResponse.AccessToken == null)
            {
                return Unauthorized();
            }
            return Ok(loginResponse);
        }
    }
}
