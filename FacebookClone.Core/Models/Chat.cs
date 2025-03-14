using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.Models
{
	public class Chat:BaseModel
	{
	 
		public Guid SenderId { get; set; }
		public AppUser Sender { get; set; }
		public Guid ReceiverId { get; set; }
		public AppUser Receiver { get; set; }
		public List<Message> Messages { get; set; } = new List<Message>();
	}

}
