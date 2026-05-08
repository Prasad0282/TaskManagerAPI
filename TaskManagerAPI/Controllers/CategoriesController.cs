using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }
    
    private int GetUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] Category category)
    {
        category.UserId = GetUserId();
        
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.Categories
            .Where(c => c.UserId == GetUserId())
            .ToListAsync();
            
        return Ok(categories);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == GetUserId());
            
        if (category == null)
            return NotFound();
            
        return Ok(category);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == GetUserId());
            
        if (category == null)
            return NotFound();
            
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}