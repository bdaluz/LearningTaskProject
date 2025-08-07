using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Services.DTOs.Tasks;
using Microsoft.AspNetCore.RateLimiting;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    public class ToDoTaskController : ControllerBase
    {
        private readonly IToDoTaskService _taskservice;

        public ToDoTaskController(IToDoTaskService toDoTaskService)
        {
            _taskservice = toDoTaskService;
        }


        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            int userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var tasks = await _taskservice.GetAllUserTasks(userid);
            
            return Ok(tasks);
        }

        //[Route("Create")]
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateDTO createDTO)
        {
            int userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var createdTask = await _taskservice.AddTask(createDTO.Title, createDTO.Description, userid);
            return StatusCode(201, createdTask);
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> EditTask(int id, [FromBody] EditDTO editDTO)
        {
            int userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!await _taskservice.TaskBelongsToUser(id, userid)) return NotFound("Cannot edit a task that doesn't exist.");

            await _taskservice.EditTask(id, editDTO.Title, editDTO.Description);
            return Ok(new { message = "Task was edited successfully." });
        }


        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveTask(int id)
        {
            int userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!await _taskservice.TaskBelongsToUser(id, userid)) return NotFound(new {message = "Task not found." });

            await _taskservice.RemoveTask(id);
            return Ok(new { message = "Task deleted successfully." });
        }

        [Route("CompleteTask/{id}")]
        [HttpPatch]
        public async Task<IActionResult> CompleteTask(int id)
        {
            int userid = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!await _taskservice.TaskBelongsToUser(id, userid)) return NotFound(new { message = "Task not found." });

            await _taskservice.MarkAsComplete(id);
            return Ok();
        }

    }
}
