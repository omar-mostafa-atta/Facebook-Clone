namespace FacebookClone.Core.Models
{
	public class Comment:BaseModel
	{
	 
		public string Text { get; set; }

		public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
		public DateTime UpdatedAt { get; set; }
		public Guid AppUserId { get; set; }
		public AppUser AppUser { get; set; }

		public Guid PostId { get; set; }
		public Post Post { get; set; }
	}
}
