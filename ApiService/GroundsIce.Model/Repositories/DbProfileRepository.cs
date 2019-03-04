namespace GroundsIce.Model.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using Dapper.Contrib.Extensions;
    using GroundsIce.Model.Abstractions;
    using GroundsIce.Model.Abstractions.Repositories;
    using GroundsIce.Model.Entities;

    public class DbProfileRepository : IProfileRepository
    {
        private const string ProfileInfoEntriesTableName = "ProfileInfoEntries";

        private const string ProfileInfoTypesTableName = "ProfileInfoTypes";

        private const string AccountsTableName = "Accounts";

        private static readonly Dictionary<string, ProfileEntryType> FromDbProfileInfoTypesMapping = new Dictionary<string, ProfileEntryType>()
        {
            { "firstname", ProfileEntryType.FirstName },
            { "lastname", ProfileEntryType.LastName },
            { "middlename", ProfileEntryType.MiddleName },
            { "description", ProfileEntryType.Description },
            { "city", ProfileEntryType.City },
            { "region", ProfileEntryType.Region },
        };

        private static readonly Dictionary<ProfileEntryType, string> ToDbProfileInfoTypesMapping =
            FromDbProfileInfoTypesMapping.ToDictionary(x => x.Value, x => x.Key);

        private readonly IConnectionFactory connectionFactory;

        public DbProfileRepository(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException("connectionFactory");
        }

        public async Task<Profile> GetProfileAsync(long userId)
        {
            if (userId < 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            using (var connection = await this.connectionFactory.GetConnectionAsync())
            {
                string sqlQuery = $"SELECT Login FROM {AccountsTableName} WHERE UserId=@UserId";
                var dbLogin = await connection.QueryAsync<string>(sqlQuery, new { UserId = userId });
                string loginValue =
                    dbLogin.Count() == 0 ? null :
                    dbLogin.Count() > 1 ? throw new DbProfileRepositoryException("Multiple profile infos were found by user id") :
                    dbLogin.First();
                if (loginValue == null)
                {
                    return null;
                }

                sqlQuery = $"SELECT p.Value, p.IsPublic, t.Type " +
                    $"FROM {ProfileInfoEntriesTableName} p, {ProfileInfoTypesTableName} t " +
                    $"WHERE p.UserId=@UserId AND t.Id=p.TypeId";
                var dbProfileEntries = await connection.QueryAsync<DbProfileEntry>(sqlQuery, new { UserId = userId });
                var profileEntries = dbProfileEntries.Select(e => new ProfileEntry
                {
                    Type = FromDbProfileInfoTypesMapping[e.Type],
                    Value = e.Value,
                    IsPublic = e.IsPublic
                }).AsList();
                return new Profile
                {
                    Login = new Login(loginValue),
                    ProfileEntriesCollection = new ProfileEntriesCollection(profileEntries)
                };
            }
        }

        public async Task<bool> SetProfileEntriesCollectionAsync(long userId, ProfileEntriesCollection profileEntriesCollection)
        {
            if (userId < 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            if (profileEntriesCollection == null)
            {
                throw new ArgumentNullException("profileEntriesCollection");
            }

            using (var connection = await this.connectionFactory.GetConnectionAsync())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    string sqlQuery = $"DELETE FROM {ProfileInfoEntriesTableName} WHERE UserId=@UserId";
                    await connection.ExecuteAsync(sqlQuery, new { UserId = userId }, transaction: transaction);
                    if (profileEntriesCollection.Count() == 0)
                    {
                        sqlQuery = $"SELECT UserId FROM {AccountsTableName} WHERE UserId=@UserId";
                        var foundUsers = await connection.QueryAsync<long>(sqlQuery, new { UserId = userId }, transaction: transaction);
                        bool userExists = foundUsers.Count() == 1;
                        if (userExists)
                        {
                            transaction.Commit();
                        }

                        return userExists;
                    }

                    var dbProfileInfoEntries = from entry in profileEntriesCollection
                                               where entry.Value != null && entry.Value.Length > 0
                                               select new DbProfileEntry
                                               {
                                                   UserId = userId,
                                                   Type = ToDbProfileInfoTypesMapping[entry.Type],
                                                   Value = entry.Value,
                                                   IsPublic = entry.IsPublic
                                               };
                    sqlQuery = $"INSERT INTO {ProfileInfoEntriesTableName} (UserId, Value, IsPublic, TypeId) " +
                        $"VALUES (@UserId, @Value, @IsPublic, (SELECT Id FROM {ProfileInfoTypesTableName} WHERE Type=@Type))";
                    int inserted = await connection.ExecuteAsync(sqlQuery, dbProfileInfoEntries, transaction: transaction);
                    bool completed = inserted == profileEntriesCollection.Count();
                    if (completed)
                    {
                        transaction.Commit();
                    }

                    return completed;
                }
                catch (SqlException ex) when (ex.Number == 547)
                {
                    return false;
                }
            }
        }

        public class DbProfileRepositoryException : Exception
        {
            public DbProfileRepositoryException(string message)
                : base(message)
            {
            }
        }

        [Table("ProfileInfoEntries")]
        private class DbProfileEntry
        {
            [Key]
            public long UserId { get; set; }

            public string Type { get; set; }

            public string Value { get; set; }

            public bool IsPublic { get; set; }
        }
    }
}
