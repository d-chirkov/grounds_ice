namespace GroundsIce.Model.ConnectionFactories
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using GroundsIce.Model.Abstractions;

    public class SqlConnectionFactory : IConnectionFactory
    {
        private readonly string connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException("connectionString");
        }

        public async Task<IDbConnection> GetConnectionAsync()
        {
            var connection = new SqlConnection(this.connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}
