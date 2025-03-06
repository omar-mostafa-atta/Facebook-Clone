using FacebookClone.Core.DTO;
using FacebookClone.Core.IRepository;
using FacebookClone.Core.Models;
using FacebookClone.EF.Repository;
using Microsoft.AspNetCore.Identity;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;
using System.Text;
using static System.Net.WebRequestMethods;


namespace FacebookClone.Core.Repository.Auhtentication
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IConfiguration _configuration;
		private readonly IEmailService _emailService;
		private readonly RedisOTPService _redisOTPService;

		public AuthenticationService(UserManager<AppUser> userManager, IConfiguration configuration, IEmailService emailService, RedisOTPService redisOTPService)
		{
			_userManager = userManager;
			_configuration = configuration;
			_emailService = emailService;
			_redisOTPService = redisOTPService;


		}
		public async Task<string> EmailOnlyToken(string email)
		{

			var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);
			List<Claim> EmailOnlyclaims = new List<Claim>();
			EmailOnlyclaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
			EmailOnlyclaims.Add(new Claim(ClaimTypes.Email, email));

			var EmailOnlytoken = new JwtSecurityToken(
			issuer: _configuration["Jwt:Issuer"],
			audience: _configuration["Jwt:Audience"],
			claims: EmailOnlyclaims,
			expires: DateTime.UtcNow.AddHours(2),
			signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
			);
			return new JwtSecurityTokenHandler().WriteToken(EmailOnlytoken);
		}
		public async Task<string> RegisterAsync(RegisterDTO registerDTO, string Otp)
		{
			var user = new AppUser
			{
				UserName = registerDTO.UserName,
				Email = registerDTO.Email,
				OTP = Otp
			};

			var result = await _userManager.CreateAsync(user, registerDTO.Password);
			if (!result.Succeeded)
			{
				return string.Join(", ", result.Errors.Select(e => e.Description));
			}

			return $"User{registerDTO.UserName} created succssefully";

		}

		public async Task<bool> VerifyOTP(SignUpResponseDto signUpResponseDto)
		{
			var OTPFromRequest = signUpResponseDto.OTP;
			var email = GetEmailFromToken(signUpResponseDto.EmailOnlyToken);
			bool isValid = await _redisOTPService.VerifyOTPAsync(email, OTPFromRequest);
			if (!isValid)
			{
				return false;
			}
			var user = await _userManager.FindByEmailAsync(email);
			user.Verify = true;
			await _userManager.UpdateAsync(user);
			return true;
		}


		public async Task<string> LoginAsync(LoginDTO loginDTO)
		{
			var user = await _userManager.FindByNameAsync(loginDTO.UserName);
			if (user == null || !await _userManager.CheckPasswordAsync(user, loginDTO.Password))
			{
				return "Wrong Username Or password";
			}
			else if (user.Verify == false)
			{
				return "Incomplete OTP process";
			}
			return await GenerateToken(user);
		}

		public async Task<string> ForgotPasswordAsync(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
				return "User not found";

			// Generate a password reset token
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			// Send the token via email
			string resetLink = $"{_configuration["FrontendBaseUrl"]}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";
			await _emailService.SendEmailAsync(email, "Password Reset Request", $"Click the link to reset your password: {resetLink}");

			return "Password reset link has been sent to your email.";
		}

		public async Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
		{
			var user = await _userManager.FindByEmailAsync(resetPasswordDTO.email);
			if (user == null)
				return "User not found";

			var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.token, resetPasswordDTO.newPassword);
			if (!result.Succeeded)
				return string.Join(", ", result.Errors.Select(e => e.Description));

			return "Password has been reset successfully.";
		}


		public async Task<string> SendOTPAsync(string email)
		{
			string otp = GenerateOTP();
			await _redisOTPService.StoreOTPAsync(email, otp);
			await _emailService.SendEmailAsync(email, "This is Your OTP", otp);
			return otp;
		}
		#region Helping Functions

		private async Task<string> GenerateToken(AppUser user)
		{
			var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);
			var userroles = await _userManager.GetRolesAsync(user);

			List<Claim> claims = new List<Claim>();

			claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));//This is token id not user id
			claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
			claims.Add(new Claim(ClaimTypes.Name, user.UserName));
			claims.Add(new Claim(ClaimTypes.Email, user.Email));
			foreach (var role in userroles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}


			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(2),
				signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
		public string GenerateOTP(int length = 6)
		{
			Random random = new Random();
			return new string(Enumerable.Repeat("0123456789", length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}
		public static string? GetEmailFromToken(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			var jwtToken = handler.ReadJwtToken(token);


			var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

			return emailClaim?.Value;
		}

		#endregion
	}
}
