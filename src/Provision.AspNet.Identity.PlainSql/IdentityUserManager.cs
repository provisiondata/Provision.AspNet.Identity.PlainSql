using System;
using Microsoft.AspNet.Identity;

namespace Provision.AspNet.Identity.PlainSql
{
	public class IdentityUserManager : UserManager<IdentityUser, Guid>
	{
		public IdentityUserManager(IUserStore<IdentityUser, Guid> store)
				: base(store)
		{
		}
	}
}
