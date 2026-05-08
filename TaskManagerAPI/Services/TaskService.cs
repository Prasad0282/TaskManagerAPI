using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.DTOs;
using TaskManagerAPI.Models.Pagination;

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
            throw new KeyNotFoundException("Project not found");
        
        // Verify category if provided
        if (taskDto.CategoryId.HasValue)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == taskDto.CategoryId && c.UserId == userId);
            if (category == null)
                throw new KeyNotFoundException("Category not found");
        }
        
        var task = new TaskItem
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            DueDate = taskDto.DueDate,
            Status = "Todo",
            Priority = taskDto.Priority ?? "Medium",
            Tags = taskDto.Tags,
            ProjectId = taskDto.ProjectId,
            CategoryId = taskDto.CategoryId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        
        return await MapToDto(task);
    }
    
    public async Task<PagedResult<TaskResponseDto>> GetUserTasksAsync(int userId, TaskFilterDto filter)
    {
        var query = _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Category)
            .Where(t => t.UserId == userId)
            .AsQueryable();
        
        // Apply filters
        if (!string.IsNullOrEmpty(filter.Status))
            query = query.Where(t => t.Status == filter.Status);
            
        if (!string.IsNullOrEmpty(filter.Priority))
            query = query.Where(t => t.Priority == filter.Priority);
            
        if (!string.IsNullOrEmpty(filter.SearchTerm))
            query = query.Where(t => 
                t.Title.Contains(filter.SearchTerm) || 
                t.Description.Contains(filter.SearchTerm));
                
        if (!string.IsNullOrEmpty(filter.Tag))
            query = query.Where(t => t.Tags != null && t.Tags.Contains(filter.Tag));
            
        if (filter.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == filter.CategoryId);
            
        if (filter.ProjectId.HasValue)
            query = query.Where(t => t.ProjectId == filter.ProjectId);
        
        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "title" => filter.SortOrder == "asc" 
                ? query.OrderBy(t => t.Title) 
                : query.OrderByDescending(t => t.Title),
            "priority" => filter.SortOrder == "asc" 
                ? query.OrderBy(t => t.Priority) 
                : query.OrderByDescending(t => t.Priority),
            "status" => filter.SortOrder == "asc" 
                ? query.OrderBy(t => t.Status) 
                : query.OrderByDescending(t => t.Status),
            "duedate" => filter.SortOrder == "asc" 
                ? query.OrderBy(t => t.DueDate) 
                : query.OrderByDescending(t => t.DueDate),
            _ => filter.SortOrder == "asc" 
                ? query.OrderBy(t => t.CreatedAt) 
                : query.OrderByDescending(t => t.CreatedAt)
        };
        
        // Get total count before pagination
        var totalCount = await query.CountAsync();
        
        // Apply pagination
        var tasks = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();
        
        var taskDtos = new List<TaskResponseDto>();
        foreach (var task in tasks)
        {
            taskDtos.Add(await MapToDto(task));
        }
        
        return new PagedResult<TaskResponseDto>
        {
            Items = taskDtos,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }
    
    public async Task<TaskResponseDto> GetTaskByIdAsync(int userId, int taskId)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            
        if (task == null)
            throw new KeyNotFoundException("Task not found");
            
        return await MapToDto(task);
    }
    
    public async Task<TaskResponseDto> UpdateTaskAsync(int userId, int taskId, TaskUpdateDto updateDto)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            
        if (task == null)
            throw new KeyNotFoundException("Task not found");
        
        // Update only provided fields
        if (updateDto.Title != null)
            task.Title = updateDto.Title;
        if (updateDto.Description != null)
            task.Description = updateDto.Description;
        if (updateDto.DueDate.HasValue)
            task.DueDate = updateDto.DueDate;
        if (updateDto.Priority != null)
            task.Priority = updateDto.Priority;
        if (updateDto.Tags != null)
            task.Tags = updateDto.Tags;
        if (updateDto.CategoryId.HasValue)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == updateDto.CategoryId && c.UserId == userId);
            if (category == null)
                throw new KeyNotFoundException("Category not found");
            task.CategoryId = updateDto.CategoryId;
        }
        
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return await GetTaskByIdAsync(userId, taskId);
    }
    
    public async Task<TaskResponseDto> UpdateTaskStatusAsync(int userId, int taskId, string status)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            
        if (task == null)
            throw new KeyNotFoundException("Task not found");
            
        var validStatuses = new[] { "Todo", "InProgress", "Done" };
        if (!validStatuses.Contains(status))
            throw new ArgumentException("Invalid status. Must be Todo, InProgress, or Done");
            
        task.Status = status;
        task.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return await GetTaskByIdAsync(userId, taskId);
    }
    
    public async Task DeleteTaskAsync(int userId, int taskId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
            
        if (task == null)
            throw new KeyNotFoundException("Task not found");
            
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }
    
    private Task<TaskResponseDto> MapToDto(TaskItem task)
    {
       var dto = new TaskResponseDto
    {
        Id = task.Id,
        Title = task.Title,
        Description = task.Description,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt,
        DueDate = task.DueDate,
        Status = task.Status,
        Priority = task.Priority,
        Tags = task.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim()).ToList() ?? new List<string>(),
        ProjectId = task.ProjectId,
        ProjectName = task.Project?.Name ?? "Unknown",
        CategoryId = task.CategoryId,
        CategoryName = task.Category?.Name
    };
       return Task.FromResult(dto);
    }
}