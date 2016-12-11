using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Provision.AspNet.Identity.PlainSql
{
	public static class Extensions
	{
		public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this IdentityUser user, UserManager<IdentityUser, Guid> manager, String authenticationType)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			return await manager.CreateIdentityAsync(user, authenticationType);
		}

		public static void AddOrCreate<TRole>(this RoleManager<TRole> manager, String name)
			where TRole : class, IRole, new()
		{
			var existing = manager.FindByName(name);
			if (existing == null) {
				var role = new TRole {
					Name = name
				};
				manager.Create(role);
			}
		}
	}
}
