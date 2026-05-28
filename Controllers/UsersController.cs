using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Dtos;
using Backend.Entities;
using Backend.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly FoodOrderDbContext _context;
        private readonly IConfiguration _config;

        public UsersController(FoodOrderDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: api/users
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AuthUserResponseDto>))]
        public async Task<ActionResult<IEnumerable<AuthUserResponseDto>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new AuthUserResponseDto
                {
                    Id = u.Id,
                    Name = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Address = u.Address,
                    Role = u.Role,
                    CreatedAt = DateTime.UtcNow
                })
                .ToListAsync();

            return Ok(users);
        }

        // POST: api/users/register
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthUserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthUserResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            var email = request.Email.Trim().ToLower();

            // Unique Email Check
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email))
            {
                return BadRequest(new { Message = "This email address is already registered." });
            }

            var user = new User
            {
                FullName = request.Name.Trim(),
                Email = email,
                Phone = request.Phone.Trim(),
                Address = request.Address.Trim(),
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                Role = "Customer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new AuthUserResponseDto
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = DateTime.UtcNow,
                Token = GenerateJwtToken(user)
            };

            return Ok(response);
        }

        // POST: api/users/login
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthUserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthUserResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var email = request.Email.Trim().ToLower();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return BadRequest(new { Message = "Email or password is incorrect." });
            }

            if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return BadRequest(new { Message = "Email or password is incorrect." });
            }

            var response = new AuthUserResponseDto
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                CreatedAt = DateTime.UtcNow,
                Token = GenerateJwtToken(user)
            };

            return Ok(response);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpireMinutes"] ?? "1440")),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
