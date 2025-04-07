using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FacebookClone.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BaseController : ControllerBase
	{

		protected async Task<IActionResult> HandleRequest(Func<Task<IActionResult>> action)
		{
			try
			{
				return await action();
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex.Message);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode(500, "An unexpected error occurred: " + ex.Message);
			}
		}

 

	}
}



