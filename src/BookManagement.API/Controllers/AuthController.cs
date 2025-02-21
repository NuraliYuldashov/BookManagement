using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using BookManagement.API.DTOs;
using BookManagement.Domain;
using Microsoft.EntityFrameworkCore;
using BookManagement.Infrastructure;
using BCrypt.Net;

namespace BookManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly BookManagementDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(BookManagementDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (string.IsNullOrEmpty(registerDto.Username) || string.IsNullOrEmpty(registerDto.Password))
            {
                return BadRequest("Username and password are required");
            }

            if (registerDto.Password.Length < 6)
            {
                return BadRequest("Password must be at least 6 characters long");
            }

            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == registerDto.Username.ToLower()))
            {
                return BadRequest("Username already exists");
            }

            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Username = user.Username
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while registering the user");
        }
    }

    /// <summary>
    /// Login with existing credentials
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Username and password are required");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => 
                u.Username.ToLower() == loginDto.Username.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password");
            }

            var token = GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                Token = token,
                Username = user.Username
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while logging in");
        }
    }

    private string GenerateJwtToken(User user)
    {
        var tokenKey = _configuration.GetSection("AppSettings:Token").Value;
        if (string.IsNullOrEmpty(tokenKey))
        {
            throw new InvalidOperationException("JWT token key is not configured");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("UserId", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
