using System;
using System.Collections.Generic;

namespace AspNet.Identity.PlainSql
{
	/// <summary>
	/// Class that represents the AspNetRoles table in the SQL Database.
	/// </summary>
	internal class RoleTable
	{
		private readonly PostgresWrapper _database;

		/// <summary>
		/// Constructor that takes a open database connection instance.
		/// </summary>
		/// <param name="database"></param>
		public RoleTable(PostgresWrapper database)
		{
			_database = database;
		}

		/// <summary>
		/// Deletes a role record from the AspNetRoles table.
		/// </summary>
		/// <param name="roleId">The role Id</param>
		/// <returns></returns>
		public int Delete(String roleId)
		{
			String commandText = "DELETE FROM \"AspNetRoles\" WHERE \"Id\" = @id";
			Dictionary<String, Object> parameters = new Dictionary<String, Object>();
			parameters.Add("@id", roleId);

			return _database.Execute(commandText, parameters);
		}

		/// <summary>
		/// Inserts a new Role record in the AspNetRoles table.
		/// </summary>
		/// <param name="role">The role's name.</param>
		/// <returns></returns>
		public int Insert(IdentityRole role)
		{
			String commandText = "INSERT INTO \"AspNetRoles\" (\"Id\", \"Name\") VALUES (@id, @name)";
			Dictionary<String, Object> parameters = new Dictionary<String, Object>();
			parameters.Add("@name", role.Name);
			parameters.Add("@id", role.Id);

			return _database.Execute(commandText, parameters);
		}

		/// <summary>
		/// Returns all role names.
		///
		/// Created by: Slawomir Figiel
		/// </summary>
		/// <returns>Role name.</returns>
		public List<IdentityRole> GetAllRoleNames()
		{
			List<IdentityRole> roles = new List<IdentityRole>();
			String commandText = "SELECT * FROM \"AspNetRoles\"";
			var rows = _database.Query(commandText, new Dictionary<String, Object>());

			foreach (var row in rows) {
				IdentityRole r = new IdentityRole(row["Name"], row["Id"]);
				roles.Add(r);
			}
			return roles;
		}

		/// <summary>
		/// Returns a role name given the roleId.
		/// </summary>
		/// <param name="roleId">The role Id.</param>
		/// <returns>Role name.</returns>
		public String GetRoleName(String roleId)
		{
			String commandText = "SELECT \"Name\" FROM \"AspNetRoles\" WHERE \"Id\" = @id";
			Dictionary<String, Object> parameters = new Dictionary<String, Object>();
			parameters.Add("@id", roleId);

			return _database.GetStrValue(commandText, parameters);
		}

		/// <summary>
		/// Returns the role Id given a role name.
		/// </summary>
		/// <param name="roleName">Role's name.</param>
		/// <returns>Role's Id.</returns>
		public String GetRoleId(String roleName)
		{
			String roleId = null;
			String commandText = "SELECT \"Id\" FROM \"AspNetRoles\" WHERE \"Name\" = @name";
			Dictionary<String, Object> parameters = new Dictionary<String, Object>() { { "@name", roleName } };

			var result = _database.QueryValue(commandText, parameters);
			if (result != null) {
				return Convert.ToString(result);
			}

			return roleId;
		}

		/// <summary>
		/// Gets the IdentityRole given the role Id.
		/// </summary>
		/// <param name="roleId"></param>
		/// <returns></returns>
		public IdentityRole GetRoleById(String roleId)
		{
			var roleName = GetRoleName(roleId);
			IdentityRole role = null;

			if (roleName != null) {
				role = new IdentityRole(roleName, roleId);
			}

			return role;
		}

		/// <summary>
		/// Gets the IdentityRole given the role name.
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public IdentityRole GetRoleByName(String roleName)
		{
			var roleId = GetRoleId(roleName);
			IdentityRole role = null;

			if (roleId != null) {
				role = new IdentityRole(roleName, roleId);
			}

			return role;
		}

		public int Update(IdentityRole role)
		{
			String commandText = "UPDATE \"AspNetRoles\" SET \"Name\" = @name WHERE \"Id\" = @id";
			Dictionary<String, Object> parameters = new Dictionary<String, Object>();
			parameters.Add("@id", role.Id);

			return _database.Execute(commandText, parameters);
		}
	}
}
