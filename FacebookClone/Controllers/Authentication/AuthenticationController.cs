using FacebookClone.Core.DTO;
using FacebookClone.Core.Models;
using FacebookClone.Core.Repository.Auhtentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FacebookClone.Controllers.Authentication
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		private readonly IAuthenticationService _auth;
		private readonly UserManager<AppUser> _userManager;
		public AuthenticationController(IAuthenticationService auth, UserManager<AppUser> userManager)
		{
			_auth = auth;

			_userManager=userManager;
		}



		[HttpPost("SignUp")]
		public async Task<IActionResult> SignUp(RegisterDTO registerFromRequest)
		{
			if (registerFromRequest == null)
				return BadRequest();

			var otp = await _auth.SendOTPAsync(registerFromRequest.Email);

			var emailToken = await _auth.EmailOnlyToken(registerFromRequest.Email);
			var accesstoken = await _auth.RegisterAsync(registerFromRequest,otp);

			return Ok(new SignUpResponseDto { EmailOnlyToken= emailToken, OTP= otp});

		}

		[HttpPost("ValidateOtp")]
		public async Task<IActionResult> ValidateOtp(SignUpResponseDto signUpResponseDto)
		{
			if (signUpResponseDto == null)
				return BadRequest();

			var result=await _auth.VerifyOTP(signUpResponseDto);
			if(result==false)
				return BadRequest("OTP Is Wrong");
		
			return Ok("OTP is correct");

		}


		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginDTO loginFromRequest)
		{
			if (loginFromRequest == null)
				return BadRequest();


			var reult = await _auth.LoginAsync(loginFromRequest);
			return Ok(new { reult });

		}

		[HttpGet("GetUserByEmail")]
		public async Task<IActionResult> ForgotPassword(string emailFromRequest) {

			 var result=_auth.ForgotPasswordAsync(emailFromRequest);

			return Ok(result);

		}

		[HttpPatch("ResetPassword")]
		public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPasswordDTO)
		{
			if(resetPasswordDTO == null)
				return BadRequest();

			var result = _auth.ResetPasswordAsync(resetPasswordDTO);

			return Ok(result);

		}
	}
}
