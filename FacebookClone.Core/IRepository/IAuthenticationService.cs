using FacebookClone.Core.DTO;

namespace FacebookClone.Core.IRepository
{
	public interface IAuthenticationService
	{
		Task<string> RegisterAsync(RegisterDTO registerDTO, string Otp);
		Task<string> LoginAsync(LoginDTO loginDTO);
		Task<string> SendOTPAsync(string email);
		Task<bool> VerifyOTP(SignUpResponseDto signUpResponseDto);
		Task<string> EmailOnlyToken(string email);
		Task<string> ForgotPasswordAsync(string email);
		Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
		Task DeleteUserAsync(string userId);
	}
}
