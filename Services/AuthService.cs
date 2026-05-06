using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using TaskManagerAPI.Models.DTOs;

namespace TaskManagerAPI.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    
    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            throw new Exception("Username already exists");
            
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            throw new Exception("Email already exists");
        
        // Create new user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        // Generate JWT token
        return GenerateJwtToken(user);
    }
    
    public async Task<string> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username);
            
        if (user == null)
            throw new Exception("Invalid username or password");
            
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Invalid username or password");
        
        return GenerateJwtToken(user);
    }
    
    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}