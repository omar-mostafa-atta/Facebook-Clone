using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.DTO
{
	public class UserProfileDto
	{
		public Guid Id { get; set; }
		public string? UserName { get; set; }
		public string? Email { get; set; }
		public string? Bio { get; set; }
		public string? ProfilePictureUrl { get; set; }
		public string? BackgroundPictureUrl { get; set; }
		public int? NumberOfFriends { get; set; }
	}
}
