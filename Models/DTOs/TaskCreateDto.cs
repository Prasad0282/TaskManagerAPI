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
    
    [Required]
    public int ProjectId { get; set; }
}