using System.Security.Claims;
using DiplomaThesis.Server.Models;
using DiplomaThesis.Shared.Commands;
using DiplomaThesis.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomaThesis.Server.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("[controller]")]
    public class AdministrationController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AdministrationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult> ListUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = new List<User>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var resultRoles = roles.Select(x => new Role { Name = x }).ToArray();
                result.Add(new User {Id = Guid.Parse(user.Id), Name = user.UserName, Roles = resultRoles});
            }
            
            return Ok(result.AsEnumerable());
        }
        
        [HttpPut("AddRole")]
        public async Task<ActionResult> AddRole(
            [FromBody] AddRoleCommand value)
        {
            if (User.IsInRole(value.RoleName)) return Ok();

            var user = await _userManager.FindByNameAsync(User.Identity!.Name);
            if (user == null) return BadRequest();

            var result = await _userManager.AddToRoleAsync(user, value.RoleName);
            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }
        
        [HttpPut("RemoveRole")]
        public async Task<ActionResult> RemoveRole(
            [FromBody] RemoveRoleCommand value)
        {
            if (!User.IsInRole(value.RoleName)) return Ok();

            var user = await _userManager.FindByNameAsync(User.Identity!.Name);
            if (user == null) return BadRequest();

            var result = await _userManager.RemoveFromRoleAsync(user, value.RoleName);
            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }
        
        [HttpDelete]
        public async Task<ActionResult> DeleteUser(
            [FromBody] DeleteUserCommand value)
        {
            var user = await _userManager.FindByNameAsync(value.UserName);
            if (user == null) return Ok();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest();

            return Ok();
        }
    }
}