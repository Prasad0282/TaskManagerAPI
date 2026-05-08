using TaskManagerAPI.Models.DTOs;
using TaskManagerAPI.Models.Pagination;

namespace TaskManagerAPI.Services;

public interface ITaskService
{
    Task<TaskResponseDto> CreateTaskAsync(int userId, TaskCreateDto taskDto);
    Task<PagedResult<TaskResponseDto>> GetUserTasksAsync(int userId, TaskFilterDto filter);
    Task<TaskResponseDto> GetTaskByIdAsync(int userId, int taskId);
    Task<TaskResponseDto> UpdateTaskAsync(int userId, int taskId, TaskUpdateDto updateDto);
    Task<TaskResponseDto> UpdateTaskStatusAsync(int userId, int taskId, string status);
    Task DeleteTaskAsync(int userId, int taskId);
}