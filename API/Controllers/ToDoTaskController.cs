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
            string? idFromClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idFromClaim == null) return BadRequest("Null");

            int userid = int.Parse(idFromClaim);
            
            if (!await _taskservice.TaskDoesExist(userid)) return BadRequest("You have no tasks");

            var tasks = await _taskservice.GetAllUserTasks(userid);
            
            return Ok(tasks);
        }

        //[Route("Create")]
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateDTO createDTO)
        {
            string? idFromClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idFromClaim == null) return BadRequest("Null");

            int userid = int.Parse(idFromClaim);

            await _taskservice.AddTask(createDTO.Title, createDTO.Description, userid);
            return Ok();
        }

        [Route("{id}")]
        [HttpPut]
        public async Task<IActionResult> EditTask(int id, [FromBody] EditDTO editDTO)
        {
            string? idFromClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idFromClaim == null) return BadRequest("Null");

            int userid = int.Parse(idFromClaim);

            if (!await _taskservice.TaskDoesExist(userid)) return BadRequest("You have no tasks");

            if (!await _taskservice.UserDoesExist(id, userid)) return NotFound("Cannot edit a task that doesn't exist.");

            await _taskservice.EditTask(id, editDTO.Title, editDTO.Description);
            return Ok("Task was edited successfully.");
        }


        [Route("{id}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveTask(int id)
        {
            string? idFromClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idFromClaim == null) return BadRequest("Null");

            int userid = int.Parse(idFromClaim);

            if (!await _taskservice.TaskDoesExist(userid)) return BadRequest("You have no tasks");

            if (!await _taskservice.UserDoesExist(id, userid)) return NotFound("Cannot remove a task that doesn't exist.");

            await _taskservice.RemoveTask(id);
            return Ok("Task deleted successfully.");
        }

        [Route("CompleteTask/{id}")]
        [HttpPatch]
        public async Task<IActionResult> CompleteTask(int id)
        {
            string? idFromClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idFromClaim == null) return BadRequest("Null");

            int userid = int.Parse(idFromClaim);

            if (!await _taskservice.TaskDoesExist(userid)) return BadRequest("You have no tasks");

            if (!await _taskservice.UserDoesExist(id, userid)) return NotFound("Cannot remove a task that doesn't exist.");

            await _taskservice.MarkAsComplete(id);
            return Ok();
        }

    }
}
