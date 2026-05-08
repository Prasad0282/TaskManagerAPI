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
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var task = await _taskService.CreateTaskAsync(GetUserId(), taskDto);
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery] TaskFilterDto filter)
    {
        var tasks = await _taskService.GetUserTasksAsync(GetUserId(), filter);
        return Ok(tasks);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(GetUserId(), id);
        return Ok(task);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var task = await _taskService.UpdateTaskAsync(GetUserId(), id, updateDto);
        return Ok(task);
    }
    
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var task = await _taskService.UpdateTaskStatusAsync(GetUserId(), id, status);
        return Ok(task);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await _taskService.DeleteTaskAsync(GetUserId(), id);
        return NoContent();
    }
}