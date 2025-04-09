using FacebookClone.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.DTO
{
	public class GetReactionsDTO
	{
		public Guid userId { get; set; }
		public ReactionType reactionType { get; set; }
	}
}
