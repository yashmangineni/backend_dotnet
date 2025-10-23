using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        // POST: api/auth/signup
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] UserDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data provided" });

            // Map DTO to User model
            var user = new User
            {
                Username = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password
            };

            if (await _db.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest(new { success = false, message = "User already exists" });

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "User registered successfully" });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Invalid data provided" });

            var user = await _db.Users.FirstOrDefaultAsync(u =>
                u.Email == request.Email && u.Password == request.Password);

            if (user == null)
                return Unauthorized(new { success = false, message = "Invalid email or password" });

            return Ok(new { success = true, message = $"Welcome {user.Username}" });
        }

        // GET: api/auth/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users.ToListAsync();
            return Ok(new { success = true, data = users });
        }
    }

    // DTO for user creation
    public class UserDto
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public record LoginRequest(string Email, string Password);
}