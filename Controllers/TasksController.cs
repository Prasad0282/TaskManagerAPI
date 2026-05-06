using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.Models.DTOs;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    
    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }
    
    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskCreateDto taskDto)
    {
        try
        {
            var task = await _taskService.CreateTaskAsync(GetUserId(), taskDto);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await _taskService.GetUserTasksAsync(GetUserId());
        return Ok(tasks);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(GetUserId(), id);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
    
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        try
        {
            var task = await _taskService.UpdateTaskStatusAsync(GetUserId(), id, status);
            return Ok(task);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        try
        {
            await _taskService.DeleteTaskAsync(GetUserId(), id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}