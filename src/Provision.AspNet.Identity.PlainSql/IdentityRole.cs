using System;
using Microsoft.AspNet.Identity;

namespace Provision.AspNet.Identity.PlainSql
{
	public class IdentityRole : IRole, IRole<String>
	{
		/// <summary>
		/// Default constructor for IdentityRole.
		/// </summary>
		public IdentityRole()
		{
			this.Id = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Constructor that takes a name as an argument.
		/// </summary>
		/// <param name="name"></param>
		public IdentityRole(String name)
				: this()
		{
			this.Name = name;
		}

		public IdentityRole(String name, String id)
		{
			this.Name = name;
			this.Id = id;
		}

		/// <summary>
		/// Role ID.
		/// </summary>
		public String Id { get; set; }

		/// <summary>
		/// Role name.
		/// </summary>
		public String Name { get; set; }
	}
}
