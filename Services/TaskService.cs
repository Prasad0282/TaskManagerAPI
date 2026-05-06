using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.DTOs;

namespace TaskManagerAPI.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    
    public TaskService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<TaskResponseDto> CreateTaskAsync(int userId, TaskCreateDto taskDto)
    {
        // Verify project belongs to user
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == taskDto.ProjectId && p.UserId == userId);
            
        if (project == null)
            throw new Exception("Project not found");
        
        var task = new TaskItem
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            DueDate = taskDto.DueDate,
            Status = "Todo",
            Priority = "Medium",
            ProjectId = taskDto.ProjectId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        
        return MapToDto(task, project.Name);
    }
    
    public async Task<List<TaskResponseDto>> GetUserTasksAsync(int userId)
    {
        var tasks = await _context.Tasks
            .Include(t => t.Project)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
            
        return tasks.Select(t => MapToDto(t, t.Project.Name)).ToList();
    }
    
    public async Task<TaskResponseDto> GetTaskByIdAsync(int userId, int taskId)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            
        if (task == null)
            throw new Exception("Task not found");
            
        return MapToDto(task, task.Project.Name);
    }
    
    public async Task<TaskResponseDto> UpdateTaskStatusAsync(int userId, int taskId, string status)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            
        if (task == null)
            throw new Exception("Task not found");
            
        var validStatuses = new[] { "Todo", "InProgress", "Done" };
        if (!validStatuses.Contains(status))
            throw new Exception("Invalid status. Must be Todo, InProgress, or Done");
            
        task.Status = status;
        await _context.SaveChangesAsync();
        
        return MapToDto(task, task.Project.Name);
    }
    
    public async Task DeleteTaskAsync(int userId, int taskId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            
        if (task == null)
            throw new Exception("Task not found");
            
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }
    
    private TaskResponseDto MapToDto(TaskItem task, string projectName)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            Status = task.Status,
            Priority = task.Priority,
            ProjectId = task.ProjectId,
            ProjectName = projectName
        };
    }
}