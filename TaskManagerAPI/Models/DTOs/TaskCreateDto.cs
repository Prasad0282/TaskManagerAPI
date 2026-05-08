using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models.DTOs;

public class TaskCreateDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    public DateTime? DueDate { get; set; }
    
    [RegularExpression("^(Low|Medium|High|Critical)$", ErrorMessage = "Priority must be Low, Medium, High, or Critical")]
    public string? Priority { get; set; } = "Medium";
    
    [StringLength(500)]
    public string? Tags { get; set; }
    
    [Required]
    public int ProjectId { get; set; }
    
    public int? CategoryId { get; set; }
}