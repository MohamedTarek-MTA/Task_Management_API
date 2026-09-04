using Microsoft.AspNetCore.Mvc;
using Task_Management_API.Application.DTOs.UserDTOs;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Application.Services;

namespace Task_Management_API.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUsersPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _userService.GetAllUsersPaged(pageNumber, pageSize);
            return Ok(users);
        }
        [HttpGet("name")]
        public async Task<IActionResult> GetAllUsersByName(
            [FromQuery]string name,
            int pageNumber = 1, 
            [FromQuery] int pageSize = 10) 
        {
            var users = await _userService.GetAllUsersByName(name, pageNumber, pageSize);
            return Ok(users);
        }
        [HttpGet("role")]
        public async Task<IActionResult> GetAllUsersByRole(
            [FromQuery] string role,
            int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var users = await _userService.GetAllUsersByRole(role, pageNumber, pageSize);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO userDTO)
        {
            var createdUser = await _userService.CreateUser(userDTO);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
        }
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDTO userDTO)
        //{
        //    var updatedUser = await _userService.UpdateUser(id, userDTO);
        //    return Ok(updatedUser);
        //}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUser(id);
            return NoContent();
        }
    }
}
