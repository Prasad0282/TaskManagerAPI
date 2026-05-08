using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models.DTOs;

public class TaskUpdateDto
{
    [StringLength(200)]
    public string? Title { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    [RegularExpression("^(Low|Medium|High|Critical)$", ErrorMessage = "Priority must be Low, Medium, High, or Critical")]
    public string? Priority { get; set; }
    
    [StringLength(500)]
    public string? Tags { get; set; }
    
    public int? CategoryId { get; set; }
}