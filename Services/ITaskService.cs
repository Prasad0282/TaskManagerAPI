using TaskManagerAPI.Models.DTOs;

namespace TaskManagerAPI.Services;

public interface ITaskService
{
    Task<TaskResponseDto> CreateTaskAsync(int userId, TaskCreateDto taskDto);
    Task<List<TaskResponseDto>> GetUserTasksAsync(int userId);
    Task<TaskResponseDto> GetTaskByIdAsync(int userId, int taskId);
    Task<TaskResponseDto> UpdateTaskStatusAsync(int userId, int taskId, string status);
    Task DeleteTaskAsync(int userId, int taskId);
}