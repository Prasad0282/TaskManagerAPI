using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.DTOs;

namespace TaskManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }
    
    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] ProjectCreateDto projectDto)
    {
        var project = new Project
        {
            Name = projectDto.Name,
            Description = projectDto.Description,
            UserId = GetUserId(),
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var userId = GetUserId();
        var projects = await _context.Projects
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.CreatedAt,
                TaskCount = p.Tasks.Count
            })
            .ToListAsync();
            
        return Ok(projects);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        var userId = GetUserId();
        var project = await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            
        if (project == null)
            return NotFound(new { message = "Project not found" });
            
        return Ok(project);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var userId = GetUserId();
        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
            
        if (project == null)
            return NotFound(new { message = "Project not found" });
            
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}