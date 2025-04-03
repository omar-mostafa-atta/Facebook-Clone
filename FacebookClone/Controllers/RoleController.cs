using FacebookClone.Core.DTO;
using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FacebookClone.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RoleController : ControllerBase
	{
		private readonly RoleManager<AppRole> _roleManager;
		private readonly UserManager<AppUser> _userManager;
		public RoleController(RoleManager<AppRole> roleManager,UserManager<AppUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create(CreateRoleDTO createRoleDTO)
		{
			if (string.IsNullOrEmpty(createRoleDTO.RoleName))
			{
				return BadRequest("Role name can not be empty");
			}


			if (await _roleManager.RoleExistsAsync(createRoleDTO.RoleName))
			{
				return BadRequest("Role already exists");
			}
			var role = new AppRole(createRoleDTO.RoleName);

			var result = await _roleManager.CreateAsync(role);
			if (!result.Succeeded)
				return BadRequest("Failed to Create role ");

			return Created();
		}

		[HttpDelete("DeleteRole")]
		public async Task<IActionResult> Delete([FromQuery] string rolename)
		{
			var role = await _roleManager.FindByNameAsync(rolename);

			if (role == null)
				return NotFound("Role not found");

			await _roleManager.DeleteAsync(role);
			return Ok($"Role {rolename} is deleted");

		}

		[HttpPost("AssignUserToRole")]
		public async Task<IActionResult> Assign(AssignRoleDTO assignRoleDTO)
		{

			var user= await _userManager.FindByIdAsync(assignRoleDTO.UserId);
			if (user == null) 
				return NotFound("User not found");
			
			var role=await _roleManager.FindByNameAsync(assignRoleDTO.RoleName);

			if (role == null)
				return NotFound("Role not found");

			await _userManager.AddToRoleAsync(user,assignRoleDTO.RoleName);
			return Ok($"User {user.UserName} is now assigned to Role {assignRoleDTO.RoleName}");
		}

		[HttpDelete("RemoveUserFromRole")]
		public async Task<IActionResult> RemoveUserFromRole([FromQuery]string roleName,[FromQuery] string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return NotFound("User not found");

			var role = await _roleManager.FindByNameAsync(roleName);

			if (role == null)
				return NotFound("Role not found");
		
			await _userManager.RemoveFromRoleAsync(user,roleName);
			return Ok("removed");


		}
	}
}
