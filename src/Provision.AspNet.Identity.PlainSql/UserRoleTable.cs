using System;
using System.Collections.Generic;

namespace AspNet.Identity.PlainSql
{
	/// <summary>
	/// Class that represents the AspNetUserRoles table in the SQL Database.
	/// </summary>
	internal class UserRolesTable
	{
		private readonly PostgresWrapper _database;

		/// <summary>
		/// Constructor that takes a open database connection instance.
		/// </summary>
		/// <param name="database"></param>
		public UserRolesTable(PostgresWrapper database)
		{
			_database = database;
		}

		/// <summary>
		/// Returns a list of user's roles.
		/// </summary>
		/// <param name="userId">The user's id.</param>
		/// <returns></returns>
		public List<String> FindByUserId(String userId)
		{
			List<String> roles = new List<String>();

			var commandText = "SELECT \"AspNetRoles\".\"Name\" FROM \"AspNetRoles\" JOIN \"AspNetUserRoles\" ON \"AspNetUserRoles\".\"RoleId\" = \"AspNetRoles\".\"Id\" WHERE \"AspNetUserRoles\".\"UserId\" = @userId;";
			var parameters = new Dictionary<String, Object>();
			parameters.Add("@userId", userId);

			var rows = _database.Query(commandText, parameters);
			foreach (var row in rows) {
				roles.Add(row["Name"]);
			}

			return roles;
		}

		/// <summary>
		/// Deletes role from a user in the AspNetUserRoles table.
		/// </summary>
		/// <param name="userId">The user's id.</param>
		/// <returns></returns>
		public int Delete(String userId, String role)
		{
			String commandText = "DELETE FROM \"AspNetUserRoles\" WHERE \"UserId\" = @userId AND \"RoleId\" = @Role;";
			Dictionary<String, Object> parameters = new Dictionary<String, Object>();
			parameters.Add("UserId", userId);
			parameters.Add("Role", role);

			return _database.Execute(commandText, parameters);
		}

		/// <summary>
		/// Deletes all roles from a user in the AspNetUserRoles table.
		///
		/// Corrected by Slawomir Figiel
		/// </summary>
		/// <param name="userId">The user's id.</param>
		/// <returns></returns>
		public int Delete(String userId)
		{
			String commandText = "DELETE FROM \"AspNetUserRoles\" WHERE \"UserId\" = @userId";
			Dictionary<String, Object> parameters = new Dictionary<String, Object>();
			parameters.Add("UserId", userId);

			return _database.Execute(commandText, parameters);
		}

		/// <summary>
		/// Inserts a new role record for a user in the UserRoles table.
		/// </summary>
		/// <param name="user">The User.</param>
		/// <param name="roleId">The Role's id.</param>
		/// <returns></returns>
		public int Insert(IdentityUser user, String roleId)
		{
			String commandText = "INSERT INTO \"AspNetUserRoles\" (\"UserId\", \"RoleId\") VALUES (@userId, @roleId)";
			Dictionary<String, Object> parameters = new Dictionary<String, Object>();
			parameters.Add("userId", user.Id);
			parameters.Add("roleId", roleId);

			return _database.Execute(commandText, parameters);
		}
	}
}
