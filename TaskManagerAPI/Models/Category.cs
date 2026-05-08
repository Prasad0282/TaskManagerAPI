using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models;

public class Category
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}