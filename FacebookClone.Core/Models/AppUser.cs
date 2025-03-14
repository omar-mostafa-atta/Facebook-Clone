using Microsoft.AspNetCore.Identity;

namespace FacebookClone.Core.Models
{
	public class AppUser:IdentityUser
	{
		public string? ProfilePictureUrl { get; set; } 
		public string? BackgroundPictureUrl { get; set; } 
		public string? ProfilePicturePublicId { get; set; }  
		public string? BackgroundPicturePublicId { get; set; }  
		public bool Verify {  get; set; }=false;
		public string? OTP {  get; set; }

		public string? Bio {  get; set; }

		public int? NumberOfFriends {  get; set; }

		public List<Post> Posts { get; set; }
		public List<SavedPosts> SavedPosts { get; set; }
		public List<Comment> Comments { get; set; }

	}
}
