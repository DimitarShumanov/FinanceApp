using FinanceApp.Auth;
using FinanceApp.Models;
using FinanceApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenService _jwtService;
        private readonly ApplicationDbContext _db;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtTokenService jwtService,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // Basic validation
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = await _userManager.FindByNameAsync(request.UserName)
                             ?? await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return Conflict(new { message = "User with same username or email already exists." });

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                DisplayName = request.DisplayName,
                Currency = request.Currency ?? "BGN",
                EmailConfirmed = true // трябва да се тества
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Код за добавяне на роля, ако се използват роли
            // await _userManager.AddToRoleAsync(user, "User");

            // create token
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var token = _jwtService.CreateToken(user, roles);

            var jwtSection = HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetSection("Jwt");
            var expireMinutes = int.Parse(jwtSection["ExpireMinutes"] ?? "60");

            return Ok(new AuthResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(expireMinutes),
                UserId = user.Id,
                UserName = user.UserName
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // find by username or email
            User user = null!;
            if (request.UserNameOrEmail.Contains("@"))
                user = await _userManager.FindByEmailAsync(request.UserNameOrEmail);
            else
                user = await _userManager.FindByNameAsync(request.UserNameOrEmail);

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials." });

            var passwordCheck = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordCheck)
                return Unauthorized(new { message = "Invalid credentials." });

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.CreateToken(user, roles);

            // може да върне грешка ако не е намерен
            var jwtSection = HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetSection("Jwt");
            var expireMinutes = int.Parse(jwtSection["ExpireMinutes"] ?? "60");

            return Ok(new AuthResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(expireMinutes),
                UserId = user.Id,
                UserName = user.UserName
            });
        }
    }
}
