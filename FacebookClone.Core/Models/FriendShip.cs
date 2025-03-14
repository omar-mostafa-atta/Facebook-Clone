namespace FacebookClone.Core.Models
{
	public class Friendship : BaseModel
	{
 

		public Guid SenderId { get; set; }
		public AppUser Sender { get; set; }  

		public Guid ReciverId { get; set; }
		public AppUser Reciver { get; set; }  

		public FriendshipStatus Status { get; set; }  

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}

	public enum FriendshipStatus
	{
		Pending,
		Accepted,
		Rejected
	}

}
