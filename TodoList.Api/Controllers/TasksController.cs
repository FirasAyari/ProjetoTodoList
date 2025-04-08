using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoList.Application.Common;
using TodoList.Application.DTOs;
using TodoList.Application.Interfaces;

namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new InvalidOperationException("User ID claim (NameIdentifier) not found in token.");
            }
            return userId;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<TaskDto>>> GetTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();
            var result = await _taskService.GetTasksAsync(userId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTaskById(long id)
        {
            var userId = GetUserId();
            var taskDto = await _taskService.GetTaskByIdAsync(userId, id);

            if (taskDto == null)
            {
                return NotFound(new { Message = $"Task with ID {id} not found or does not belong to the user." });
            }

            return Ok(taskDto);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var createdTask = await _taskService.AddTaskAsync(userId, createTaskDto);

            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(long id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var success = await _taskService.UpdateTaskAsync(userId, id, updateTaskDto);

            if (!success)
            {
                return NotFound(new { Message = $"Task with ID {id} not found or could not be updated." });
            }

            return NoContent();
        }

        [HttpPut("{id}/completion")]
        public async Task<IActionResult> SetTaskCompletion(long id, [FromBody] SetCompletionDto completionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var success = await _taskService.SetTaskCompletionAsync(userId, id, completionDto.IsCompleted);

            if (!success)
            {
                return NotFound(new { Message = $"Task with ID {id} not found or could not be updated." });
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            var userId = GetUserId();
            var success = await _taskService.DeleteTaskAsync(userId, id);

            if (!success)
            {
                return NotFound(new { Message = $"Task with ID {id} not found or could not be deleted." });
            }

            return NoContent();
        }
    }
}