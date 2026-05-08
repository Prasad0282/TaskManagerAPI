using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace TaskManagerAPI.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var (statusCode, response) = exception switch
        {
            KeyNotFoundException => (
                (int)HttpStatusCode.NotFound,
                new { error = "Not Found", message = exception.Message }
            ),
            UnauthorizedAccessException => (
                (int)HttpStatusCode.Unauthorized,
                new { error = "Unauthorized", message = exception.Message }
            ),
            ArgumentException => (
                (int)HttpStatusCode.BadRequest,
                new { error = "Bad Request", message = exception.Message }
            ),
            ValidationException => (
                (int)HttpStatusCode.BadRequest,
                new { error = "Validation Error", message = exception.Message }
            ),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                new { error = "Internal Server Error", message = "An unexpected error occurred. Please try again later." }
            )
        };
        
        context.Response.StatusCode = statusCode;
        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });
        await context.Response.WriteAsync(jsonResponse);
    }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}