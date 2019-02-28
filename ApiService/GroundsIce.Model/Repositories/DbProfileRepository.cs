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

        public async Task<Profile_OLD> GetProfileAsync(long userId)
        {
            if (userId < 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            using (var connection = await this.connectionFactory.GetConnectionAsync())
            {
                string sqlQuery = $"SELECT Login FROM {AccountsTableName} WHERE UserId=@UserId";
                var dbLogin = await connection.QueryAsync<string>(sqlQuery, new { UserId = userId });
                string login =
                    dbLogin.Count() == 0 ? null :
                    dbLogin.Count() > 1 ? throw new DbProfileRepositoryException("Multiple profile infos were found by user id") :
                    dbLogin.First();
                if (login == null)
                {
                    return null;
                }

                sqlQuery = $"SELECT p.Value, p.IsPublic, t.Type " +
                    $"FROM {ProfileInfoEntriesTableName} p, {ProfileInfoTypesTableName} t " +
                    $"WHERE p.UserId=@UserId AND t.Id=p.TypeId";
                var dbProfileInfoEntries = await connection.QueryAsync<DbProfileInfoEntry>(sqlQuery, new { UserId = userId });
                var profileInfo = from dbEntry in dbProfileInfoEntries
                                  select new ProfileEntry
                                  {
                                      Type = FromDbProfileInfoTypesMapping[dbEntry.Type],
                                      Value = dbEntry.Value,
                                      IsPublic = dbEntry.IsPublic
                                  };
                return new Profile_OLD
                {
                    Login = login,
                    Avatar = null,
                    ProfileInfo = profileInfo.ToList()
                };
            }
        }

        public async Task<bool> SetProfileInfoAsync(long userId, List<ProfileEntry> profileInfo)
        {
            if (userId < 0)
            {
                throw new ArgumentOutOfRangeException("userId");
            }

            if (profileInfo == null)
            {
                throw new ArgumentNullException("profileInfo");
            }

            using (var connection = await this.connectionFactory.GetConnectionAsync())
            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    string sqlQuery = $"DELETE FROM {ProfileInfoEntriesTableName} WHERE UserId=@UserId";
                    await connection.ExecuteAsync(sqlQuery, new { UserId = userId }, transaction: transaction);
                    if (profileInfo.Count() == 0)
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

                    var dbProfileInfoEntries = from entry in profileInfo
                                               where entry.Value != null && entry.Value.Length > 0
                                               select new DbProfileInfoEntry
                                               {
                                                   UserId = userId,
                                                   Type = ToDbProfileInfoTypesMapping[entry.Type],
                                                   Value = entry.Value,
                                                   IsPublic = entry.IsPublic
                                               };
                    sqlQuery = $"INSERT INTO {ProfileInfoEntriesTableName} (UserId, Value, IsPublic, TypeId) " +
                        $"VALUES (@UserId, @Value, @IsPublic, (SELECT Id FROM {ProfileInfoTypesTableName} WHERE Type=@Type))";
                    int inserted = await connection.ExecuteAsync(sqlQuery, dbProfileInfoEntries, transaction: transaction);
                    bool completed = inserted == profileInfo.Count();
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
        private class DbProfileInfoEntry
        {
            [Key]
            public long UserId { get; set; }

            public string Type { get; set; }

            public string Value { get; set; }

            public bool IsPublic { get; set; }
        }
    }
}
