using FacebookClone.Core.DTO;
using FacebookClone.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.IRepository
{
	public interface ICommentRepository
	{
		Task Create(AddCommentDTO addCommentDTO, AppUser user);
	}
}
