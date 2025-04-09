
namespace FacebookClone.Core.Models
{
	public class SavedPosts
	{
		public DateTime SavedAt { get; set; }= DateTime.UtcNow;
		public Guid AppUserId { get; set; }
		public AppUser AppUser { get; set; }

		public Guid PostId { get; set; }
		public Post Post { get; set; }
	}
}
