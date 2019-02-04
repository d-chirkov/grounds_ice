using Dapper.Contrib.Extensions;
using GroundsIce.Model.Abstractions.Repositories;
using GroundsIce.Model.Abstractions;
using GroundsIce.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;

namespace GroundsIce.Model.Repositories
{
	public class DbAccountRepositoryException : Exception
	{
		public DbAccountRepositoryException(string message) : base(message)
		{
		}
	}

	public class DbAccountRepository : IAccountRepository
	{
		[Table("Accounts")]
		class DbAccount
		{
			[Key]
			public long UserId { get; set; }
			public string Login { get; set; }
			public string Password { get; set; }
		}

		private IConnectionFactory _connectionFactory;
		private const string _accountsTableName = "Accounts";
		private const string _profileInfoTableName = "ProfileInfo";

		public DbAccountRepository(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory ?? throw new ArgumentNullException("connectionFactory");
		}

		public async Task<Account> CreateAccountAsync(string login, string password)
		{
			if (login == null) throw new ArgumentNullException("login");
			if (password == null) throw new ArgumentNullException("password");
			using (var connection = await _connectionFactory.GetConnectionAsync())
			{
				try
				{
					long userId = connection.Insert(new DbAccount { Login = login, Password = password });
					return new Account(userId, login);
				}
				catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
				{
					return null;
				}
			}
		}

		public async Task<Account> GetAccountAsync(string login, string password)
		{
			if (login == null) throw new ArgumentNullException("login");
			if (password == null) throw new ArgumentNullException("password");
			string sqlQuery = $"SELECT * FROM {_accountsTableName} WHERE Login=@Login AND Password=@Password";
			return await GetAccountBySqlParamsAsync(sqlQuery, new { Login = login, Password = password });
		}

		public async Task<Account> GetAccountAsync(long userId)
		{
			if (userId < 0) throw new ArgumentOutOfRangeException("userId");
			string sqlQuery = $"SELECT * FROM {_accountsTableName} WHERE UserId=@UserId";
			return await GetAccountBySqlParamsAsync(sqlQuery, new { UserId = userId });
		}

		private async Task<Account> GetAccountBySqlParamsAsync<T>(string sql, T sqlParams)
		{
			using (var connection = await _connectionFactory.GetConnectionAsync())
			{
				var accounts = await connection.QueryAsync<DbAccount>(sql, sqlParams);
				DbAccount dbAccount = 
					accounts.Count() == 0 ? null :
					accounts.Count() > 1 ? throw new DbAccountRepositoryException("Multiple accounts were found by login and password") :
					accounts.FirstOrDefault() ?? throw new DbAccountRepositoryException("Null account was returned");
				return dbAccount != null ? new Account(dbAccount.UserId, dbAccount.Login) : null;
			}
		}

		public async Task<bool> ChangeLoginAsync(long userId, string newLogin)
		{
			if (userId < 0) throw new ArgumentOutOfRangeException("userId");
			if (newLogin == null) throw new ArgumentNullException("newLogin");
			using (var connection = await _connectionFactory.GetConnectionAsync())
			{
				try
				{
					string sqlQuery = $"UPDATE {_accountsTableName} SET Login = @NewLogin WHERE UserId = @UserId";
					int changedRows = await connection.ExecuteAsync(sqlQuery, new { UserId = userId, NewLogin = newLogin });
					return changedRows == 1 ? true : throw new DbAccountRepositoryException($"Couldn't change login without conflicts");
				}
				catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
				{
					return false;
				}
			}
		}

		public async Task<bool> ChangePasswordAsync(long userId, string oldPassword, string newPassword)
		{
			if (userId < 0) throw new ArgumentOutOfRangeException("userId");
			if (oldPassword == null) throw new ArgumentNullException("oldPassword");
			if (newPassword == null) throw new ArgumentNullException("newPassword");
			string sqlQuery = $"UPDATE {_accountsTableName} SET Password = @NewPassword WHERE UserId = @UserId AND Password = @OldPassword";
			using (var connection = await _connectionFactory.GetConnectionAsync())
			{
				int changedRows = await connection.ExecuteAsync(sqlQuery, 
					new { UserId = userId, OldPassword = oldPassword, NewPassword = newPassword });
				return changedRows == 1;
			}
		}
	}
}
