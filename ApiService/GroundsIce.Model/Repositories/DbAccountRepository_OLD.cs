namespace GroundsIce.Model.Repositories
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using GroundsIce.Model.Abstractions;
    using GroundsIce.Model.Abstractions.Repositories;
    using GroundsIce.Model.Entities;

    public class DbAccountRepository_OLD : IAccountRepository_OLD
    {
        private const string AccountsTableName = "Accounts";
        private const string ProfileInfoTableName = "ProfileInfo";
        private readonly IConnectionFactory connectionFactory;

        public DbAccountRepository_OLD(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException("connectionFactory");
        }

        public async Task<Account_OLD> CreateAccountAsync(string login, string password)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            using (var connection = await this.connectionFactory.GetConnectionAsync())
            {
                try
                {
                    long userId = connection.Insert(new DbAccount { Login = login, Password = password });
                    return new Account_OLD(userId, login);
                }
                catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
                {
                    return null;
                }
            }
        }

        public async Task<Account_OLD> GetAccountAsync(string login, string password)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            string sqlQuery = $"SELECT * FROM {AccountsTableName} WHERE Login=@Login AND Password=@Password";
            return await this.GetAccountBySqlParamsAsync(sqlQuery, new { Login = login, Password = password });
        }

        public async Task<Account_OLD> GetAccountAsync(long userId)
        {
            if (userId < 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            string sqlQuery = $"SELECT * FROM {AccountsTableName} WHERE UserId=@UserId";
            return await this.GetAccountBySqlParamsAsync(sqlQuery, new { UserId = userId });
        }

        public async Task<bool> ChangeLoginAsync(long userId, string newLogin)
        {
            if (userId < 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            if (newLogin == null)
            {
                throw new ArgumentNullException("newLogin");
            }

            using (var connection = await this.connectionFactory.GetConnectionAsync())
            {
                try
                {
                    string sqlQuery = $"UPDATE {AccountsTableName} SET Login = @NewLogin WHERE UserId = @UserId";
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
            if (userId < 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            if (oldPassword == null)
            {
                throw new ArgumentNullException("oldPassword");
            }

            if (newPassword == null)
            {
                throw new ArgumentNullException("newPassword");
            }

            string sqlQuery = $"UPDATE {AccountsTableName} SET Password = @NewPassword WHERE UserId = @UserId AND Password = @OldPassword";
            using (var connection = await this.connectionFactory.GetConnectionAsync())
            {
                int changedRows = await connection.ExecuteAsync(
                    sqlQuery,
                    new { UserId = userId, OldPassword = oldPassword, NewPassword = newPassword });
                return changedRows == 1;
            }
        }

        private async Task<Account_OLD> GetAccountBySqlParamsAsync<T>(string sql, T sqlParams)
        {
            using (var connection = await this.connectionFactory.GetConnectionAsync())
            {
                var accounts = await connection.QueryAsync<DbAccount>(sql, sqlParams);
                DbAccount dbAccount =
                    accounts.Count() == 0 ? null :
                    accounts.Count() > 1 ? throw new DbAccountRepositoryException("Multiple accounts were found by login and password") :
                    accounts.FirstOrDefault() ?? throw new DbAccountRepositoryException("Null account was returned");
                return dbAccount != null ? new Account_OLD(dbAccount.UserId, dbAccount.Login) : null;
            }
        }

        public class DbAccountRepositoryException : Exception
        {
            public DbAccountRepositoryException(string message)
                : base(message)
            {
            }
        }

        [Table("Accounts")]
        private class DbAccount
        {
            [Key]
            public long UserId { get; set; }

            public string Login { get; set; }

            public string Password { get; set; }
        }
    }
}
