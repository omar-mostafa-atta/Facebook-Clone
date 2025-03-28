using FacebookClone.Core.Models;
 

namespace FacebookClone.Core.DTO
{
	public class AddReactionDTO
	{
		public ReactionType ReactionType { get; set; }
		public string PostId { get; set; }
	}
}
