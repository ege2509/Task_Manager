using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementApi.Data;
using TaskManagementApi.DTOs;
using TaskManagementApi.Models;

namespace TaskManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET /api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasks([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var userId = GetUserId();

            var query = _context.Tasks.Where(t => t.UserId == userId);
            
            if (from.HasValue) query = query.Where(t => t.DueDate >= from);
            if (to.HasValue) query = query.Where(t => t.DueDate <= to);

            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId)
                .Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(tasks);
        }

        // GET /api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponseDto>> GetTask(int id)
        {
            var userId = GetUserId();

            var task = await _context.Tasks
                .Where(t => t.Id == id && t.UserId == userId)
                .Select(t => new TaskResponseDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (task is null)
                return NotFound();

            return Ok(task);
        }

        // POST /api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> CreateTask(CreateTaskDto dto)
        {
            var userId = GetUserId();

            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status ?? "Pending",
                DueDate = dto.DueDate,
                UserId = userId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var response = new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt
            };

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, response);
        }

        // PUT /api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto dto)
        {
            var userId = GetUserId();

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task is null)
                return NotFound();

            task.Title = dto.Title ?? task.Title;
            task.Description = dto.Description ?? task.Description;
            task.Status = dto.Status ?? task.Status;
            task.DueDate = dto.DueDate ?? task.DueDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetUserId();

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task is null)
                return NotFound();

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}