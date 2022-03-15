using System.Security.Claims;
using DiplomaThesis.Server.Data;
using DiplomaThesis.Server.Models;
using DiplomaThesis.Shared.Commands;
using DiplomaThesis.Shared.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserGroup = DiplomaThesis.Server.Models.UserGroup;

namespace DiplomaThesis.Server.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("[controller]/[action]")]
    public class AdministrationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly ApplicationDbContext _context;

        public AdministrationController(
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult> GetUser(
            [FromRoute] Guid userId
            )
        {
            var user = await _context.Users.FindAsync(userId.ToString());

            if (user is null) return NotFound();
            
            var roles = await _userManager.GetRolesAsync(user);
            var resultRoles = roles.Select(x => new RoleContract { Name = x });
            var result = new UserContract
            {
                Id = Guid.Parse(user.Id),
                Name = user.UserName,
                UserGroupId = user.UserGroup?.Id ?? Guid.Empty,
                Roles = resultRoles
            };
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult> ListUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = new List<UserContract>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var resultRoles = roles.Select(x => new RoleContract { Name = x });
                result.Add(new UserContract
                {
                    Id = Guid.Parse(user.Id),
                    Name = user.UserName,
                    UserGroupId = user.UserGroup?.Id ?? Guid.Empty,
                    Roles = resultRoles
                });
            }
            
            return Ok(result.AsEnumerable());
        }
        
        [HttpGet]
        public ActionResult ListRoles()
        {
            var result = _roleManager.Roles;
            
            return Ok(result.AsEnumerable());
        }
        
        [HttpPut]
        public async Task<ActionResult> AddRole(
            [FromBody] AddRoleCommand addRoleCommand)
        {
            var checkRoleExists = await _roleManager.RoleExistsAsync(addRoleCommand.RoleName);
            if (!checkRoleExists) return BadRequest();
            
            var user = await _userManager.FindByNameAsync(addRoleCommand.UserName);
            if (user is null) return NotFound();

            var checkUserHasRole = await _userManager.IsInRoleAsync(user, addRoleCommand.RoleName);
            if (checkUserHasRole) return Ok();

            var result = await _userManager.AddToRoleAsync(user, addRoleCommand.RoleName);
            if (!result.Succeeded) return BadRequest();

            return Ok();
        }
        
        [HttpPut]
        public async Task<ActionResult> RemoveRole(
            [FromBody] RemoveRoleCommand removeRoleCommand)
        {
            if (removeRoleCommand.RoleName == "Admin") return BadRequest();

            var user = await _userManager.FindByNameAsync(removeRoleCommand.UserName);
            if (user is null) return NotFound();

            var checkUserHasRole = await _userManager.IsInRoleAsync(user, removeRoleCommand.RoleName);
            if (!checkUserHasRole) return Ok();
            
            var result = await _userManager.RemoveFromRoleAsync(user, removeRoleCommand.RoleName);
            if (!result.Succeeded) return BadRequest();

            return Ok();
        }
        
        [HttpDelete]
        public async Task<ActionResult> DeleteUser(
            [FromBody] DeleteUserCommand deleteUserCommand)
        {
            var user = await _userManager.FindByNameAsync(deleteUserCommand.UserName);
            if (user is null) return NotFound();

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (loggedInUserId.Equals(user.Id)) return BadRequest();

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return StatusCode(500);

            return Ok();
        }
        
        [HttpGet]
        public ActionResult ListUserGroups()
        {
            var result = _context.UserGroups.Select(group => new UserGroupContract
            {
                Id = group.Id,
                Name = group.Name,
                Users = group.Users.Select(user => Guid.Parse(user.Id))
            }).ToList();
            
            return Ok(result.AsEnumerable());
        }
        
        [HttpPost]
        public ActionResult CreateUserGroup(
            [FromBody] CreateUserGroupCommand createUserGroupCommand)
        {
            if (_context.UserGroups.Any(group => group.Name.Equals(createUserGroupCommand.Name)))
            {
                return BadRequest();
            }

            var userGroup = new UserGroup
            {
                Id = Guid.NewGuid(),
                Name = createUserGroupCommand.Name,
                Users = new List<ApplicationUser>()
            };

            var result = _context.UserGroups.Add(userGroup);
            _context.SaveChanges();
            
            return Ok(new UserGroupContract
            {
                Id = result.Entity.Id,
                Name = result.Entity.Name,
                Users = new List<Guid>()
            });
        }
        
        [HttpDelete]
        public ActionResult DeleteUserGroup(
            [FromRoute] Guid userGroupId)
        {
            var userGroup = _context.UserGroups.Find(userGroupId.ToString());
            if (userGroup is null) return NotFound();

            _context.UserGroups.Remove(userGroup);
            _context.SaveChanges();
            
            return Ok();
        }
        
        [HttpPut]
        public ActionResult MoveUserToUserGroup(
            [FromBody] MoveUserToUserGroupCommand moveUserToUserGroupCommand)
        {
            var user = _context.Users.Find(moveUserToUserGroupCommand.UserId.ToString());
            if (user is null) return NotFound();

            if (moveUserToUserGroupCommand.UserGroupId.Equals(Guid.Empty))
            {
                user.UserGroup = null;
            }
            else
            {
                var userGroup = _context.UserGroups.Find(moveUserToUserGroupCommand.UserGroupId);
                if (userGroup is null) return NotFound();
                
                user.UserGroup = userGroup;
            }

            _context.SaveChanges();
            
            return Ok();
        }
    }
}