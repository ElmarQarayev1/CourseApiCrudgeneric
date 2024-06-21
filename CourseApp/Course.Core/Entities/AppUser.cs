using System;
using Microsoft.AspNetCore.Identity;

namespace Course.Core.Entities
{
	public class AppUser:IdentityUser
	{
		public string FullName { get; set; }
	}
}

