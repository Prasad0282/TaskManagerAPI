using TaskManagerAPI.Models.DTOs;

namespace TaskManagerAPI.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<string> LoginAsync(LoginRequest request);
}