
namespace FacebookClone.Core.Models
{
	public class SavedPosts
	{
		public DateTime SavedAt { get; set; }= DateTime.UtcNow;
		public int AppUserId { get; set; }
		public AppUser AppUser { get; set; }

		public int PostId { get; set; }
		public Post Post { get; set; }
	}
}
