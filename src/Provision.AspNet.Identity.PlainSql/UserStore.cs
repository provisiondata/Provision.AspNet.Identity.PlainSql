using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Provision.AspNet.Identity.PlainSql
{
	/// <summary>
	/// Class that implements the key ASP.NET Identity user store interfaces
	/// </summary>
	public class UserStore : IUserLoginStore<IdentityUser, Guid>,
					IUserClaimStore<IdentityUser, Guid>,
					IUserRoleStore<IdentityUser, Guid>,
					IUserPasswordStore<IdentityUser, Guid>,
					IUserSecurityStampStore<IdentityUser, Guid>,
					IQueryableUserStore<IdentityUser, Guid>,
					IUserEmailStore<IdentityUser, Guid>
	{
		private readonly UserTable<IdentityUser> _userTable;
		private readonly RoleTable _roleTable;
		private readonly UserRolesTable _userRolesTable;
		private readonly UserClaimsTable _userClaimsTable;
		private readonly UserLoginsTable _userLoginsTable;

		/// <summary>
		/// Constructor that takes a PostgreSQLDatabase as argument.
		/// </summary>
		/// <param name="connection"></param>
		public UserStore(IDbConnection connection)
		{
			var database = new SqlDatabase(connection);
			_userTable = new UserTable<IdentityUser>(database);
			_roleTable = new RoleTable(database);
			_userRolesTable = new UserRolesTable(database);
			_userClaimsTable = new UserClaimsTable(database);
			_userLoginsTable = new UserLoginsTable(database);
		}

		public IQueryable<IdentityUser> Users {
			get {
				return _userTable.GetAllUsers().AsQueryable();
			}
		}

		/// <summary>
		/// Insert a new IdentityUser in the AspNetUserTable.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task CreateAsync(IdentityUser user)
		{
			if (user == null) {
				throw new ArgumentNullException(nameof(user));
			}

			_userTable.Insert(user);

			return Task.FromResult<Object>(null);
		}

		/// <summary>
		/// Returns an IdentityUser instance based on a userId query.
		/// </summary>
		/// <param name="userId">The user's Id.</param>
		/// <returns></returns>
		public Task<IdentityUser> FindByIdAsync(Guid userId)
		{
			var result = _userTable.GetUserById(userId) as IdentityUser;
			if (result != null) {
				return Task.FromResult<IdentityUser>(result);
			}

			return Task.FromResult<IdentityUser>(null);
		}

		/// <summary>
		/// Returns an IdentityUser instance based on a userName query.
		/// </summary>
		/// <param name="userName">The user's name.</param>
		/// <returns></returns>
		public Task<IdentityUser> FindByNameAsync(String userName)
		{
			if (String.IsNullOrEmpty(userName)) {
				throw new ArgumentException("Null or empty argument: userName");
			}

			List<IdentityUser> result = _userTable.GetUserByName(userName) as List<IdentityUser>;

			if (result != null) {
				if (result.Count == 1) {
					return Task.FromResult<IdentityUser>(result[0]);
				} else if (result.Count > 1) {
					//todo: exception for release mode?
#if DEBUG
					throw new ArgumentException("More than one user record returned.");
#endif
				}
			}

			return Task.FromResult<IdentityUser>(null);
		}

		/// <summary>
		/// Updates the AspNetUsersTable with the IdentityUser instance values.
		/// </summary>
		/// <param name="user">IdentityUser to be updated.</param>
		/// <returns></returns>
		public Task UpdateAsync(IdentityUser user)
		{
			if (user == null) {
				throw new ArgumentNullException(nameof(user));
			}

			_userTable.Update(user);

			return Task.FromResult<Object>(null);
		}

		/// <summary>
		/// Inserts a claim to the AspNetUserClaims table for the given user.
		/// </summary>
		/// <param name="user">User to have claim added.</param>
		/// <param name="claim">Claim to be added.</param>
		/// <returns></returns>
		public Task AddClaimAsync(IdentityUser user, Claim claim)
		{
			if (user == null) {
				throw new ArgumentNullException(nameof(user));
			}

			if (claim == null) {
				throw new ArgumentNullException(nameof(user));
			}

			_userClaimsTable.Insert(claim, user.Id);

			return Task.FromResult<Object>(null);
		}

		/// <summary>
		/// Returns all claims for a given user.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task<IList<Claim>> GetClaimsAsync(IdentityUser user)
		{
			var identity = _userClaimsTable.FindByUserId(user.Id);

			return Task.FromResult<IList<Claim>>(identity.Claims.ToList());
		}

		/// <summary>
		/// Removes a claim from a user.
		/// </summary>
		/// <param name="user">User to have claim removed.</param>
		/// <param name="claim">Claim to be removed.</param>
		/// <returns></returns>
		public Task RemoveClaimAsync(IdentityUser user, Claim claim)
		{
			if (user == null) {
				throw new ArgumentNullException(nameof(user));
			}

			if (claim == null) {
				throw new ArgumentNullException(nameof(claim));
			}

			_userClaimsTable.Delete(user, claim);

			return Task.FromResult<Object>(null);
		}

		/// <summary>
		/// Inserts a Login in the AspNetUserLogins table for a given User.
		/// </summary>
		/// <param name="user">User to have login added.</param>
		/// <param name="login">Login to be added.</param>
		/// <returns></returns>
		public Task AddLoginAsync(IdentityUser user, UserLoginInfo login)
		{
			if (user == null) {
				throw new ArgumentNullException(nameof(user));
			}

			if (login == null) {
				throw new ArgumentNullException(nameof(login));
			}

			_userLoginsTable.Insert(user, login);

			return Task.FromResult<Object>(null);
		}

		/// <summary>
		/// Returns an IdentityUser based on the Login info.
		/// </summary>
		/// <param name="login"></param>
		/// <returns></returns>
		public Task<IdentityUser> FindAsync(UserLoginInfo login)
		{
			if (login == null) {
				throw new ArgumentNullException(nameof(login));
			}

			var userId = _userLoginsTable.FindUserIdByLogin(login);
			IdentityUser user = _userTable.GetUserById(userId) as IdentityUser;
			if (user != null) {
				return Task.FromResult<IdentityUser>(user);
			}

			return Task.FromResult<IdentityUser>(null);
		}

		/// <summary>
		/// Returns list of UserLoginInfo for a given IdentityUser.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task<IList<UserLoginInfo>> GetLoginsAsync(IdentityUser user)
		{
			if (user == null) {
				throw new ArgumentNullException(nameof(user));
			}

			List<UserLoginInfo> logins = _userLoginsTable.FindByUserId(user.Id);
			if (logins != null) {
				return Task.FromResult<IList<UserLoginInfo>>(logins);
			}

			return Task.FromResult<IList<UserLoginInfo>>(null);
		}

		/// <summary>
		/// Deletes a login from AspNetUserLogins table for a given IdentityUser.
		/// </summary>
		/// <param name="user">User to have login removed.</param>
		/// <param name="login">Login to be removed.</param>
		/// <returns></returns>
		public Task RemoveLoginAsync(IdentityUser user, UserLoginInfo login)
		{
			if (user == null) {
				throw new ArgumentNullException(nameof(user));
			}

			if (login == null) {
				throw new ArgumentNullException(nameof(login));
			}

			_userLoginsTable.Delete(user, login);

			return Task.FromResult<Object>(null);
		}

		/// <summary>
		/// Inserts a entry in the AspNetUserRoles table.
		/// </summary>
		/// <param name="user">User to have role added.</param>
		/// <param name="roleName">Name of the role to be added to user.</param>
		/// <returns></returns>
		public Task AddToRoleAsync(IdentityUser user, String roleName)
		{
			if (user == null) {
				throw new ArgumentNullException(nameof(user));
			}

			if (String.IsNullOrEmpty(roleName)) {
				throw new ArgumentException("Argument cannot be null or empty: roleName.");
			}

			String roleId = _roleTable.GetRoleId(roleName);
			if (!String.IsNullOrEmpty(roleId)) {
				_userRolesTable.Insert(user, roleId);
			}

			return Task.FromResult<Object>(null);
		}

		/// <summary>
		/// Returns the roles for a given IdentityUser.
		/// </summary>
		/// <param name="user">IdentityUser Object.</param>
		/// <returns></returns>
		public Task<IList<String>> GetRolesAsync(IdentityUser user)
		{
			if (user == null) {
				throw new ArgumentNullException(nameof(user));
			}

			List<String> roles = _userRolesTable.FindByUserId(user.Id);
			{
				if (roles != null) {
					return Task.FromResult<IList<String>>(roles);
				}
			}

			return Task.FromResult<IList<String>>(null);
		}

		/// <summary>
		/// Verifies if a user is in a role.
		/// </summary>
		/// <param name="user">IdentityUser Object.</param>
		/// <param name="role">Role String.</param>
		/// <returns></returns>
		public Task<bool> IsInRoleAsync(IdentityUser user, String role)
		{
			if (user == null) {
				throw new ArgumentNullException(nameof(user));
			}

			if (String.IsNullOrEmpty(role)) {
				throw new ArgumentNullException(nameof(role));
			}

			List<String> roles = _userRolesTable.FindByUserId(user.Id);
			{
				if (roles != null && roles.Contains(role)) {
					return Task.FromResult<bool>(true);
				}
			}

			return Task.FromResult<bool>(false);
		}

		/// <summary>
		/// Removes a user from a role.
		///
		/// Created by Slawomir Figiel
		/// </summary>
		/// <param name="user">IdentityUser Object.</param>
		/// <param name="role">Role String.</param>
		/// <returns></returns>
		public Task RemoveFromRoleAsync(IdentityUser user, String role)
		{
			if (user == null) {
				throw new ArgumentNullException(nameof(user));
			}

			if (role == null) {
				throw new ArgumentNullException(nameof(role));
			}

			String roleId = _roleTable.GetRoleId(role);
			if (!String.IsNullOrEmpty(roleId)) {
				_userRolesTable.Delete(user.Id, roleId);
			}

			return Task.FromResult<Object>(null);
		}

		/// <summary>
		/// Deletes a user.
		/// </summary>
		/// <param name="user">IdentityUser Object.</param>
		/// <returns></returns>
		public Task DeleteAsync(IdentityUser user)
		{
			if (user != null) {
				_userTable.Delete(user);
			}

			return Task.FromResult<Object>(null);
		}

		/// <summary>
		/// Returns the PasswordHash for a given IdentityUser.
		/// </summary>
		/// <param name="user">IdentityUser Object.</param>
		/// <returns></returns>
		public Task<String> GetPasswordHashAsync(IdentityUser user)
		{
			String passwordHash = _userTable.GetPasswordHash(user.Id);

			return Task.FromResult<String>(passwordHash);
		}

		/// <summary>
		/// Verifies if user has password.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task<bool> HasPasswordAsync(IdentityUser user)
		{
			var hasPassword = !String.IsNullOrEmpty(_userTable.GetPasswordHash(user.Id));

			return Task.FromResult<bool>(Boolean.Parse(hasPassword.ToString()));
		}

		/// <summary>
		/// Sets the password hash for a given IdentityUser.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="passwordHash"></param>
		/// <returns></returns>
		public Task SetPasswordHashAsync(IdentityUser user, String passwordHash)
		{
			user.PasswordHash = passwordHash;

			return Task.FromResult<Object>(null);
		}

		/// <summary>
		///  Set security stamp.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="stamp"></param>
		/// <returns></returns>
		public Task SetSecurityStampAsync(IdentityUser user, String stamp)
		{
			user.SecurityStamp = stamp;

			return Task.FromResult(0);
		}

		/// <summary>
		/// Get security stamp.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task<String> GetSecurityStampAsync(IdentityUser user)
		{
			return Task.FromResult(user.SecurityStamp);
		}

		/// <summary>
		/// Set email on user.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="email"></param>
		/// <returns></returns>
		public Task SetEmailAsync(IdentityUser user, String email)
		{
			user.Email = email;
			_userTable.Update(user);

			return Task.FromResult(0);
		}

		/// <summary>
		/// Get email from user.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task<String> GetEmailAsync(IdentityUser user)
		{
			return Task.FromResult(user.Email);
		}

		/// <summary>
		/// Get if user email is confirmed.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Task<bool> GetEmailConfirmedAsync(IdentityUser user)
		{
			return Task.FromResult(user.EmailConfirmed);
		}

		/// <summary>
		/// Set when user email is confirmed.
		/// </summary>
		/// <param name="user"></param>
		/// <param name="confirmed"></param>
		/// <returns></returns>
		public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed)
		{
			user.EmailConfirmed = confirmed;
			_userTable.Update(user);

			return Task.FromResult(0);
		}

		/// <summary>
		/// Get user by email.
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		public Task<IdentityUser> FindByEmailAsync(String email)
		{
			if (String.IsNullOrEmpty(email)) {
				throw new ArgumentNullException(nameof(email));
			}

			List<IdentityUser> result = _userTable.GetUserByEmail(email) as List<IdentityUser>;
			if (result != null && result.Count > 0) {
				return Task.FromResult<IdentityUser>(result[0]);
			}

			return Task.FromResult<IdentityUser>(null);
		}

		private bool _disposed ;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed) {
				if (disposing) {
					// Dispose managed resources
				}

				// Free unmanaged resources
				// Set large fields to null.

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}
}
