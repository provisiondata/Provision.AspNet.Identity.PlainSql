using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNet.Identity.PlainSql
{
	/// <summary>
	/// Class that represents the AspNetUsers table in the SQL database.
	/// </summary>
	internal class UserTable<TUser>
			where TUser : IdentityUser, new()
	{
		private readonly PostgresWrapper _database;

		/// <summary>
		/// Constructor that takes a SQL database instance.
		/// </summary>
		/// <param name="database"></param>
		public UserTable(PostgresWrapper database)
		{
			_database = database;
		}

		/// <summary>
		/// Gets the user's name, provided with an ID.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public String GetUserName(String userId)
		{
			var commandText = "SELECT \"UserName\" FROM \"AspNetUsers\" WHERE \"Id\" = @id";
			var parameters = new Dictionary<String, Object>() { { "@id", userId } };
			return _database.GetStrValue(commandText, parameters);
		}

		/// <summary>
		/// Gets the user's ID, provided with a user name.
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public String GetUserId(String userName)
		{
			if (String.IsNullOrWhiteSpace(userName))
				throw new ArgumentNullException(nameof(userName));

			//Due to SQL's case sensitivity, we have another column for the user name in lowercase.
			userName = userName.ToLower(CultureInfo.InvariantCulture);

			var commandText = "SELECT \"Id\" FROM \"AspNetUsers\" WHERE LOWER(\"UserName\") = @name";
			var parameters = new Dictionary<String, Object>() { { "@name", userName } };

			return _database.GetStrValue(commandText, parameters);
		}

		/// <summary>
		/// Returns all users.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<TUser> GetAllUsers()
		{
			var users = new List<TUser>();
			var commandText = "SELECT * FROM \"AspNetUsers\"";
			var rows = _database.Query(commandText, new Dictionary<String, Object>());

			foreach (var row in rows) {
				TUser user = new TUser();
				user.Id = row["Id"];
				user.UserName = row["UserName"];
				user.PasswordHash = String.IsNullOrEmpty(row["PasswordHash"]) ? null : row["PasswordHash"];
				user.SecurityStamp = String.IsNullOrEmpty(row["SecurityStamp"]) ? null : row["SecurityStamp"];
				user.Email = String.IsNullOrEmpty(row["Email"]) ? null : row["Email"];
				user.EmailConfirmed = row["EmailConfirmed"] == "True";
				users.Add(user);
			}

			return users;
		}

		public TUser GetUserById(String userId)
		{
			TUser user = null;
			var commandText = "SELECT * FROM \"AspNetUsers\" WHERE \"Id\" = @id";
			var parameters = new Dictionary<String, Object>() { { "@id", userId } };

			var rows = _database.Query(commandText, parameters);
			if (rows != null && rows.Count == 1) {
				var row = rows[0];
				user = new TUser();
				user.Id = row["Id"];
				user.UserName = row["UserName"];
				user.PasswordHash = String.IsNullOrEmpty(row["PasswordHash"]) ? null : row["PasswordHash"];
				user.SecurityStamp = String.IsNullOrEmpty(row["SecurityStamp"]) ? null : row["SecurityStamp"];
				user.Email = String.IsNullOrEmpty(row["Email"]) ? null : row["Email"];
				user.EmailConfirmed = row["EmailConfirmed"] == "True";
			}

			return user;
		}

		/// <summary>
		/// Returns a list of TUser instances given a user name.
		/// </summary>
		/// <param name="userName">User's name.</param>
		/// <returns></returns>
		public IEnumerable<TUser> GetUserByName(String userName)
		{
			if (String.IsNullOrWhiteSpace(userName))
				throw new ArgumentNullException(nameof(userName));

			//Due to SQL's case sensitivity, we have another column for the user name in lowercase.
			userName = userName.ToLower(CultureInfo.InvariantCulture);

			var users = new List<TUser>();
			var commandText = "SELECT * FROM \"AspNetUsers\" WHERE LOWER(\"UserName\") = @name";
			var parameters = new Dictionary<String, Object>() { { "@name", userName } };
			var rows = _database.Query(commandText, parameters);
			foreach (var row in rows) {
				TUser user = new TUser();
				user.Id = row["Id"];
				user.UserName = row["UserName"];
				user.PasswordHash = String.IsNullOrEmpty(row["PasswordHash"]) ? null : row["PasswordHash"];
				user.SecurityStamp = String.IsNullOrEmpty(row["SecurityStamp"]) ? null : row["SecurityStamp"];
				user.Email = String.IsNullOrEmpty(row["Email"]) ? null : row["Email"];
				user.EmailConfirmed = row["EmailConfirmed"] == "True";
				users.Add(user);
			}

			return users;
		}

		/// <summary>
		/// Returns a list of TUser instances given a user email.
		/// </summary>
		/// <param name="email">User's email address.</param>
		/// <returns></returns>
		public IEnumerable<TUser> GetUserByEmail(String email)
		{
			if (String.IsNullOrWhiteSpace(email))
				throw new ArgumentNullException(nameof(email));

			//Due to SQL's case sensitivity, we have another column for the user name in lowercase.
			email = email.ToLower(CultureInfo.InvariantCulture);

			var users = new List<TUser>();
			var commandText = "SELECT * FROM \"AspNetUsers\" WHERE LOWER(\"Email\") = @email";
			var parameters = new Dictionary<String, Object>() { { "@email", email } };

			var rows = _database.Query(commandText, parameters);
			foreach (var row in rows) {
				TUser user = new TUser();
				user.Id = row["Id"];
				user.UserName = row["UserName"];
				user.PasswordHash = String.IsNullOrEmpty(row["PasswordHash"]) ? null : row["PasswordHash"];
				user.SecurityStamp = String.IsNullOrEmpty(row["SecurityStamp"]) ? null : row["SecurityStamp"];
				user.Email = String.IsNullOrEmpty(row["Email"]) ? null : row["Email"];
				user.EmailConfirmed = row["EmailConfirmed"] == "True";
				users.Add(user);
			}

			return users;
		}

		/// <summary>
		/// Return the user's password hash.
		/// </summary>
		/// <param name="userId">The user's id.</param>
		/// <returns></returns>
		public String GetPasswordHash(String userId)
		{
			if (String.IsNullOrWhiteSpace(userId))
				throw new ArgumentNullException(nameof(userId));

			var commandText = "SELECT \"PasswordHash\" FROM \"AspNetUsers\" WHERE \"Id\" = @id";
			var parameters = new Dictionary<String, Object>();
			parameters.Add("@id", userId);
			var passHash = _database.GetStrValue(commandText, parameters);
			if (String.IsNullOrEmpty(passHash)) {
				return null;
			}

			return passHash;
		}

		/// <summary>
		/// Sets the user's password hash.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="passwordHash"></param>
		/// <returns></returns>
		public Int32 SetPasswordHash(String userId, String passwordHash)
		{
			if (String.IsNullOrWhiteSpace(userId))
				throw new ArgumentNullException(nameof(userId));

			if (String.IsNullOrWhiteSpace(passwordHash))
				throw new ArgumentNullException(nameof(passwordHash));

			var commandText = "UPDATE \"AspNetUsers\" SET \"PasswordHash\" = @pwdHash WHERE \"Id\" = @id";
			var parameters = new Dictionary<String, Object>();
			parameters.Add("@pwdHash", passwordHash);
			parameters.Add("@id", userId);

			return _database.Execute(commandText, parameters);
		}

		/// <summary>
		/// Returns the user's security stamp.
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public String GetSecurityStamp(String userId)
		{
			if (String.IsNullOrWhiteSpace(userId))
				throw new ArgumentNullException(nameof(userId));

			var commandText = "SELECT \"SecurityStamp\" FROM \"AspNetUsers\" WHERE \"Id\" = @id";
			var parameters = new Dictionary<String, Object>() { { "@id", userId } };
			var result = _database.GetStrValue(commandText, parameters);

			return result;
		}

		/// <summary>
		/// Inserts a new user in the AspNetUsers table.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Int32 Insert(TUser user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			String commandText = @"
				INSERT INTO ""AspNetUsers""(""Id"", ""UserName"", ""PasswordHash"", ""SecurityStamp"", ""Email"", ""EmailConfirmed"")
				VALUES (@id, @name, @pwdHash, @SecStamp, @email, @emailconfirmed);";

			var parameters = new Dictionary<String, Object>();
			parameters.Add("@name", user.UserName);
			parameters.Add("@id", user.Id);
			parameters.Add("@pwdHash", user.PasswordHash);
			parameters.Add("@SecStamp", user.SecurityStamp);
			parameters.Add("@email", user.Email);
			parameters.Add("@emailconfirmed", user.EmailConfirmed);

			return _database.Execute(commandText, parameters);
		}

		/// <summary>
		/// Deletes a user from the AspNetUsers table.
		/// </summary>
		/// <param name="userId">The user's id.</param>
		/// <returns></returns>
		private Int32 Delete(String userId)
		{
			if (String.IsNullOrWhiteSpace(userId))
				throw new ArgumentNullException(nameof(userId));

			var commandText = "DELETE FROM \"AspNetUsers\" WHERE \"Id\" = @userId";
			var parameters = new Dictionary<String, Object>();
			parameters.Add("@userId", userId);

			return _database.Execute(commandText, parameters);
		}

		/// <summary>
		/// Deletes a user from the AspNetUsers table.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Int32 Delete(TUser user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			return Delete(user.Id);
		}

		/// <summary>
		/// Updates a user in the AspNetUsers table.
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public Int32 Update(TUser user)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			var commandText = "UPDATE \"AspNetUsers\" SET \"UserName\" = @userName, \"PasswordHash\" = @pswHash, \"SecurityStamp\" = @secStamp, \"Email\"= @email, \"EmailConfirmed\" = @emailconfirmed WHERE \"Id\" = @userId;";
			var parameters = new Dictionary<String, Object>();
			parameters.Add("@userName", user.UserName);
			parameters.Add("@pswHash", user.PasswordHash);
			parameters.Add("@secStamp", user.SecurityStamp);
			parameters.Add("@userId", user.Id);
			parameters.Add("@email", user.Email);
			parameters.Add("@emailconfirmed", user.EmailConfirmed);

			return _database.Execute(commandText, parameters);
		}
	}
}
