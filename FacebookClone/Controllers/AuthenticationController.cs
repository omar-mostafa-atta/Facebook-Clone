using FacebookClone.Core.DTO;
using FacebookClone.Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace FacebookClone.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : BaseController
	{
		private readonly Core.IRepository.IAuthenticationService _auth;
	
		public AuthenticationController(Core.IRepository.IAuthenticationService auth)
		{
			_auth = auth;
		}



		[HttpPost("SignUp")]
		public async Task<IActionResult> SignUp(RegisterDTO registerFromRequest)
		{
			var otp = await _auth.SendOTPAsync(registerFromRequest.Email);

			var emailToken = await _auth.EmailOnlyToken(registerFromRequest.Email);
			var accesstoken = await _auth.RegisterAsync(registerFromRequest, otp);

			return Ok(new SignUpResponseDto { EmailOnlyToken = emailToken, OTP = otp });

		}

		[HttpPost("ValidateOtp")]
		public async Task<IActionResult> ValidateOtp(SignUpResponseDto signUpResponseDto)
		{
			var result = await _auth.VerifyOTP(signUpResponseDto);
			if (result == false)
				return BadRequest("Invalid OTP");

			return Ok("OTP is correct");

		}


		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginDTO loginFromRequest)
		{
			if (loginFromRequest == null)
				return BadRequest("Invalid login request");

			var result = await _auth.LoginAsync(loginFromRequest);
			if (result == "Wrong Username Or password" || result == "Incomplete OTP process")
				return Unauthorized(result);

			
			Response.Cookies.Append("jwt", result, new CookieOptions
			{
				HttpOnly = true, 
				Secure = true,   // 34an HTTPS
				SameSite = SameSiteMode.Strict,
				Expires = DateTimeOffset.UtcNow.AddHours(2), 
				Path = "/"       // bt5li el cookie available lkol el paths
			});

			
			return Ok(new { message = "Login successful" });
		}


		[HttpGet("ForgotPassword/{email}")]
		public async Task<IActionResult> ForgotPassword(string email)
		{

			var result = await _auth.ForgotPasswordAsync(email);

			return Ok(result);

		}

		[HttpPatch("ResetPassword")]
		public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordDTO resetPasswordDTO)
		{
			if (resetPasswordDTO == null)
				return BadRequest();

			var result =await _auth.ResetPasswordAsync(resetPasswordDTO);

			return Ok(result);

		}

		[HttpDelete("Delete/{userId}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteUser(string userId)
		{
			try
			{
				await _auth.DeleteUserAsync(userId);
				return Ok("User is deleted");
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(ex);
			}
			catch (Exception ex)
			{
				return Forbid(ex.Message);
			}
		}
	}
}
