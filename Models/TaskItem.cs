using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerAPI.Models;

public class TaskItem
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? DueDate { get; set; }
    
    [Required]
    public string Status { get; set; } = "Todo"; // Todo, InProgress, Done
    
    [Required]
    public string Priority { get; set; } = "Medium"; // Low, Medium, High
    
    // Foreign Keys
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    
    // Navigation properties
    [ForeignKey("ProjectId")]
    public Project Project { get; set; } = null!;
    
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}