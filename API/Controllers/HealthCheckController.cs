using Microsoft.AspNetCore.Mvc;

namespace Task_Management_API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("Health check passed.");
        }
    }
}
