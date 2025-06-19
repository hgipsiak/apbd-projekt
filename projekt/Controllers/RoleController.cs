using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace projekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role name is required.");
            }
            
            var exists = await _roleManager.RoleExistsAsync(roleName);
            if (exists)
            {
                return BadRequest("Role already exists.");
            }
            
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (result.Succeeded)
            {
                return Ok(new { Message = $"Role '{roleName}' created successfully." });
            }
            
            return BadRequest(result.Errors);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignUserToRole(string roleName, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return BadRequest("Role does not exist.");
            }
            
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok(new { Message = $"Role '{roleName}' assigned to user '{email}' successfully." });
            }
            
            return BadRequest(result.Errors);
        }
    }
}
