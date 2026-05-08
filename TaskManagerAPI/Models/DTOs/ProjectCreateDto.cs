using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models.DTOs;

public class ProjectCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
}