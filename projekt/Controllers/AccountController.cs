using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using projekt.DTOs;

namespace projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var user = new IdentityUser()
            {
                UserName = dto.Login,
                Email = dto.Email
            };
            
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                return Ok(new { Message = "User registered successfully." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            
            return BadRequest(ModelState);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(
                dto.Login,
                dto.Password,
                true,
                false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return Ok(new { Message = "Login successful." });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return BadRequest(new { Message = "User account locked out." });
            }
            
            return BadRequest(new { Message = "Invalid login or password." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return Ok(new { Message = "Logout successful." });
        }
    }
}
