using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace AspNet.Identity.PlainSql
{
	/// <summary>
	/// Class that encapsulates SQL database connections and CRUD operations.
	/// </summary>
	internal class PostgresWrapper
	{
		private readonly IDbConnection _connection;

		/// <summary>
		/// Constructor which takes the database connection.
		/// </summary>
		/// <param name="connection">The database connection.</param>
		public PostgresWrapper(IDbConnection connection)
		{
			_connection = connection;
		}

		/// <summary>
		/// Executes a non-query SQL statement.
		/// </summary>
		/// <param name="commandText">The SQL query to execute.</param>
		/// <param name="parameters">Optional parameters to pass to the query.</param>
		/// <returns>The count of records affected by the SQL statement.</returns>
		public int Execute(String commandText, Dictionary<String, Object> parameters)
		{
			int result = 0;

			if (String.IsNullOrEmpty(commandText)) {
				throw new ArgumentException("Command text cannot be null or empty.");
			}

			try {
				OpenConnection();
				var command = CreateCommand(commandText, parameters);
				result = command.ExecuteNonQuery();
			}
			finally {
				_connection.Close();
			}

			return result;
		}

		/// <summary>
		/// Executes a SQL query that returns a single scalar value as the result.
		/// </summary>
		/// <param name="commandText">The SQL query to execute.</param>
		/// <param name="parameters">Optional parameters to pass to the query.</param>
		/// <returns></returns>
		public Object QueryValue(String commandText, Dictionary<String, Object> parameters)
		{
			Object result = null;

			if (String.IsNullOrEmpty(commandText)) {
				throw new ArgumentException("Command text cannot be null or empty.");
			}

			try {
				OpenConnection();
				var command = CreateCommand(commandText, parameters);
				result = command.ExecuteScalar();
			}
			finally {
				CloseConnection();
			}

			return result;
		}

		/// <summary>
		/// Executes a SQL query that returns a list of rows as the result.
		/// </summary>
		/// <param name="commandText">The SQL query to execute.</param>
		/// <param name="parameters">Parameters to pass to the SQL query.</param>
		/// <returns>A list of a Dictionary of Key, values pairs representing the ColumnName and corresponding value.</returns>
		public IList<IDictionary<String, String>> Query(String commandText, Dictionary<String, Object> parameters)
		{
			IList<IDictionary<String, String>> rows = null;
			if (String.IsNullOrEmpty(commandText)) {
				throw new ArgumentException("Command text cannot be null or empty.");
			}

			try {
				OpenConnection();
				var command = CreateCommand(commandText, parameters);
				using (var reader = command.ExecuteReader()) {
					rows = new List<IDictionary<String, String>>();
					while (reader.Read()) {
						var row = new Dictionary<String, String>();
						for (var i = 0; i < reader.FieldCount; i++) {
							var columnName = reader.GetName(i);
							var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
							row.Add(columnName, columnValue);
						}
						rows.Add(row);
					}
				}
			}
			finally {
				CloseConnection();
			}

			return rows;
		}

		/// <summary>
		/// Creates a NpgsqlCommand Object with the given parameters.
		/// </summary>
		/// <param name="commandText">The SQL query to execute.</param>
		/// <param name="parameters">Parameters to pass to the SQL query.</param>
		/// <returns></returns>
		private IDbCommand CreateCommand(String commandText, Dictionary<String, Object> parameters)
		{
			var command = _connection.CreateCommand();
			command.CommandText = commandText;
			AddParameters(command, parameters);

			return command;
		}

		/// <summary>
		/// Adds the parameters to a SQL command.
		/// </summary>
		/// <param name="command">The NpgsqlCommand command to execute.</param>
		/// <param name="parameters">Parameters to pass to the SQL query.</param>
		private static void AddParameters(IDbCommand command, Dictionary<String, Object> parameters)
		{
			if (parameters == null) {
				return;
			}

			foreach (var param in parameters) {
				var parameter = command.CreateParameter();
				parameter.ParameterName = param.Key;
				parameter.Value = param.Value ?? DBNull.Value;
				command.Parameters.Add(parameter);
			}
		}

		/// <summary>
		/// Helper method to return query a String value.
		/// </summary>
		/// <param name="commandText">The SQL query to execute.</param>
		/// <param name="parameters">Parameters to pass to the SQL query.</param>
		/// <returns>The String value resulting from the query.</returns>
		public String GetStrValue(String commandText, Dictionary<String, Object> parameters)
		{
			String value = QueryValue(commandText, parameters) as String;
			return value;
		}

		/// <summary>
		/// Opens a connection if not open.
		/// </summary>
		private void OpenConnection()
		{
			var retries = 10;
			if (_connection.State == ConnectionState.Open) {
				return;
			}

			while (retries >= 0 && _connection.State != ConnectionState.Open) {
				_connection.Open();
				retries--;
				Thread.Sleep(50);
			}
		}

		/// <summary>
		/// Closes the connection if it is open.
		/// </summary>
		public void CloseConnection()
		{
			if (_connection.State == ConnectionState.Open) {
				_connection.Close();
			}
		}
	}
}
