using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.DTOs;
using TaskManagerAPI.Services;
using Xunit;
using FluentAssertions;

namespace TaskManagerAPI.Tests.Services;

public class TaskServiceTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        return new AppDbContext(options);
    }
    
    [Fact]
    public async Task CreateTaskAsync_ValidTask_ReturnsTaskDto()
    {
        // Arrange
        using var context = CreateContext();
        
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash"
        };
        
        var project = new Project
        {
            Id = 1,
            Name = "Test Project",
            UserId = 1
        };
        
        context.Users.Add(user);
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        
        var service = new TaskService(context);
        var taskDto = new TaskCreateDto
        {
            Title = "Test Task",
            Description = "Test Description",
            ProjectId = 1,
            Priority = "High",
            Tags = "test,important"
        };
        
        // Act
        var result = await service.CreateTaskAsync(1, taskDto);
        
        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Test Task");
        result.Priority.Should().Be("High");
        result.Status.Should().Be("Todo");
        result.Tags.Should().Contain(new[] { "test", "important" });
    }
    
    [Fact]
    public async Task GetUserTasksAsync_WithFilters_ReturnsFilteredResults()
    {
        // Arrange
        using var context = CreateContext();
        
        var user = new User { Id = 1, Username = "test", Email = "test@test.com", PasswordHash = "hash" };
        var project = new Project { Id = 1, Name = "Project", UserId = 1 };
        
        context.Users.Add(user);
        context.Projects.Add(project);
        
        var tasks = new List<TaskItem>
        {
            new() { Title = "High Priority", Priority = "High", Status = "Todo", ProjectId = 1, UserId = 1 },
            new() { Title = "Low Priority", Priority = "Low", Status = "Done", ProjectId = 1, UserId = 1 },
            new() { Title = "Medium Priority", Priority = "Medium", Status = "Todo", ProjectId = 1, UserId = 1 }
        };
        
        context.Tasks.AddRange(tasks);
        await context.SaveChangesAsync();
        
        var service = new TaskService(context);
        
        // Act
        var highPriorityFilter = new TaskFilterDto { Priority = "High" };
        var result = await service.GetUserTasksAsync(1, highPriorityFilter);
        
        // Assert
        result.Items.Should().HaveCount(1);
        result.Items[0].Title.Should().Be("High Priority");
        result.TotalCount.Should().Be(1);
    }
    
    [Fact]
    public async Task UpdateTaskStatusAsync_InvalidStatus_ThrowsException()
    {
        // Arrange
        using var context = CreateContext();
        
        var user = new User { Id = 1, Username = "test", Email = "test@test.com", PasswordHash = "hash" };
        var project = new Project { Id = 1, Name = "Project", UserId = 1 };
        var task = new TaskItem { Id = 1, Title = "Task", Status = "Todo", ProjectId = 1, UserId = 1 };
        
        context.Users.Add(user);
        context.Projects.Add(project);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();
        
        var service = new TaskService(context);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UpdateTaskStatusAsync(1, 1, "InvalidStatus"));
    }
}