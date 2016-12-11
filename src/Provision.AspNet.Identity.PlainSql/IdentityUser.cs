using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Provision.AspNet.Identity.PlainSql
{
	/// <summary>
	/// Class that implements the ASP.NET Identity IUser interface.
	/// </summary>
	public class IdentityUser : IUser<Guid>
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public IdentityUser()
		{
			Id = Guid.NewGuid();
		}

		/// <summary>
		/// Constructor that takes user name as argument.
		/// </summary>
		/// <param name="userName"></param>
		public IdentityUser(String userName)
						: this()
		{
			UserName = userName;
		}

		/// <summary>
		/// User ID.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// User's name.
		/// </summary>
		public String UserName { get; set; }

		/// <summary>
		/// Email.
		/// </summary>
		public virtual String Email { get; set; }

		/// <summary>
		/// True if the email is confirmed, default is false.
		/// </summary>
		public virtual bool EmailConfirmed { get; set; }

		/// <summary>
		/// The salted/hashed form of the user password.
		/// </summary>
		public virtual String PasswordHash { get; set; }

		/// <summary>
		/// A random value that should change whenever a users credentials have changed (password changed, login removed).
		/// </summary>
		public virtual String SecurityStamp { get; set; }

		/// <summary>
		///
		/// </summary>
		public IEnumerable<UserLoginInfo> Logins { get; set; }
	}
}
