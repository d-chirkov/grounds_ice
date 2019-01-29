using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroundsIce.Model.Abstractions;

namespace GroundsIce.Model.ConnectionFactories
{
	public class SqlConnectionFactory : IConnectionFactory
	{
		private string _connectionString;

		public SqlConnectionFactory(string connectionString)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException("connectionString");
		}

		public async Task<IDbConnection> GetConnectionAsync()
		{
			var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync();
			return connection;
		}
	}
}
