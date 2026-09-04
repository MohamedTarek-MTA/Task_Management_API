using Microsoft.AspNetCore.Mvc;
using Task_Management_API.Application.DTOs.ProjectDTOs;
using Task_Management_API.Application.Interfaces;
using Task_Management_API.Application.Services;

namespace Task_Management_API.API.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService projectService) 
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var projects = await _projectService.GetAllProjectsPaged(pageNumber, pageSize);
            return Ok(projects);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDTO projectDTO)
        {
            var createdProject = await _projectService.CreateProject(projectDTO);
            return CreatedAtAction(nameof(GetProjectById), new { id = createdProject.Id }, createdProject);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectDTO projectDTO)
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }
            await _projectService.UpdateProject(id, projectDTO);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _projectService.GetProjectById(id);
            if (project == null)
            {
                return NotFound();
            }
            await _projectService.DeleteProject(id);
            return NoContent();
        }
    }
}
