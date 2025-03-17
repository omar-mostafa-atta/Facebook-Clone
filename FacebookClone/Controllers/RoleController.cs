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
		public RoleController(RoleManager<AppRole> roleManager)
		{
			_roleManager = roleManager;
		}

		[HttpPost("Create")]
		public async Task<IActionResult> Create([FromBody]string roleName)
		{
			if (string.IsNullOrEmpty(roleName))
			{

				return BadRequest("Role name can not be empty");
			}


			if (await _roleManager.RoleExistsAsync(roleName))
			{

				return BadRequest("Role already exists");
			}


			var role = new AppRole(roleName);

			var result = await _roleManager.CreateAsync(role);
			if (!result.Succeeded)
				return BadRequest("Failed to ");

			return Created();
		}
	}
}
