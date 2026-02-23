using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.DTOs;
using TaskManagementApi.Models;
using System.IdentityModel.Tokens.Jwt;    
using System.Security.Claims;                
using System.Text;                           
using Microsoft.IdentityModel.Tokens;        
using BCrypt.Net;


namespace TaskManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration; 
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register(Register dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            {
                return BadRequest("Username already exists");
            }

            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return BadRequest("Email already exists");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var dbUser = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash
            };

            _context.Users.Add(dbUser);
            await _context.SaveChangesAsync();

            var response = new UserResponse
            {
                Id = dbUser.Id,
                Username = dbUser.Username,
                Email = dbUser.Email,
                CreatedAt = dbUser.CreatedAt
            };

            return CreatedAtAction(nameof(Register), new { id = dbUser.Id }, response);
        }

        [HttpPost("login")]  
        public async Task<ActionResult<LoginResponse>> Login(Login dto)
        {
            var dbUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (dbUser == null)
            {
                return BadRequest("Invalid Username");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, dbUser.PasswordHash);

            if (!isPasswordValid)
            {
                return BadRequest("Invalid Password");
            }

            var token = GenerateJwtToken(dbUser);

            var response = new LoginResponse
            {
                Token = token,
                User = new UserResponse
                {
                    Id = dbUser.Id,
                    Username = dbUser.Username,
                    Email = dbUser.Email,
                    CreatedAt = dbUser.CreatedAt
                }
            };

            return Ok(response);
        }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
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
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    }
}