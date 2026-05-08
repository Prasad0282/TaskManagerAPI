namespace TaskManagerAPI.Models.DTOs;

public class TaskFilterDto
{
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? SearchTerm { get; set; }
    public string? Tag { get; set; }
    public int? CategoryId { get; set; }
    public int? ProjectId { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public string? SortOrder { get; set; } = "desc"; // asc or desc
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}