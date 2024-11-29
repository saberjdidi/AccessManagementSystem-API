using AccessManagementSystem_API.Dtos;
using AccessManagementSystem_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccessManagementSystem_API.Controllers
{
    [Authorize(Roles = "Admin, Manager, User")]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto roleDto)
        {
            if (string.IsNullOrEmpty(roleDto.RoleName))
            {
                return BadRequest(new { message = "Role Name is required" });
            }

            var roleExist = await _roleManager.RoleExistsAsync(roleDto.RoleName);
            if (roleExist)
            {
                return BadRequest(new { message = "Role already exist" });
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(roleDto.RoleName));
            if (result.Succeeded) { 
             return Ok(new { message = "Role created successfully" });
            }

            return BadRequest("Role creation failed");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            // Fetch all roles from the database
            var roles = await _roleManager.Roles.ToListAsync();

            // Build the response by iterating over the roles
            var roleResponseList = new List<RoleResponseDto>();
            foreach (var role in roles)
            {
                var totalUsers = (await _userManager.GetUsersInRoleAsync(role.Name!)).Count;
                roleResponseList.Add(new RoleResponseDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    TotalUsers = totalUsers
                });
            }

            return Ok(roleResponseList);

            //var roles = await _roleManager.Roles.Select(r => new RoleResponseDto {
            //    Id = r.Id,
            //    Name = r.Name,
            //    TotalUsers = _userManager.GetUsersInRoleAsync(r.Name!).Result.Count
            //}).ToListAsync();

            //return Ok(roles);
        }

        [HttpDelete("role/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if(role is null)
            {
                return NotFound(new { message = "Role not found" });
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded) {
                return Ok(new { message = "Role deleted successfully" });
            }

            return BadRequest(new { message = "Role deletion failed" });
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRole([FromBody] RoleAssignDto roleAssignDto)
        {
            var user = await _userManager.FindByIdAsync(roleAssignDto.UserId);
            if (user is null)
            {
                return NotFound(new { message = "User not found" });
            }

            var role = await _roleManager.FindByIdAsync(roleAssignDto.RoleId);
            if (role is null)
            {
                return NotFound(new { message = "Role not found" });
            }

            var result = await _userManager.AddToRoleAsync(user, role.Name!);
            if (result.Succeeded)
            {
                return Ok(new { message = "Role assigned successfully" });
            }

            var error = result.Errors.FirstOrDefault();
            return BadRequest(error!.Description);
        }
    }
}
