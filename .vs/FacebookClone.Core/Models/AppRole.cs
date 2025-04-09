using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.Core.Models
{
	public class AppRole:IdentityRole<Guid>
	{
		public AppRole() : base()
		{
		}

		public AppRole(string roleName) : base(roleName)
		{
		}

		 
	}
}
