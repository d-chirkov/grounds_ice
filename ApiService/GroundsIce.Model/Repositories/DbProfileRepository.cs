using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using GroundsIce.Model.Abstractions;
using GroundsIce.Model.Abstractions.Repositories;
using GroundsIce.Model.Entities;

namespace GroundsIce.Model.Repositories
{
	public class DbProfileRepositoryException : Exception
	{
		public DbProfileRepositoryException(string message) : base(message)
		{
		}
	}

	public class DbProfileRepository : IProfileRepository
	{
		private const string _profileInfoEntriesTableName = "ProfileInfoEntries";
		private const string _profileInfoTypesTableName = "ProfileInfoTypes";
		private const string _accountsTableName = "Accounts";

		[Table("ProfileInfoEntries")]
		class DbProfileInfoEntry
		{
			[Key]
			public long UserId { get; set; }
			public string Type { get; set; }
			public string Value { get; set; }
			public bool IsPublic { get; set; }
		}

		static private Dictionary<string, ProfileInfoType> _fromDbProfileInfoTypesMapping = new Dictionary<string, ProfileInfoType>()
		{
			{ "firstname", ProfileInfoType.FirstName },
			{ "lastname", ProfileInfoType.LastName },
			{ "middlename", ProfileInfoType.MiddleName },
			{ "location", ProfileInfoType.Location },
			{ "description", ProfileInfoType.Description },
		};
		static private Dictionary<ProfileInfoType, string> _toDbProfileInfoTypesMapping =
			_fromDbProfileInfoTypesMapping.ToDictionary(x => x.Value, x => x.Key);

		private IConnectionFactory _connectionFactory;

		public DbProfileRepository(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory ?? throw new ArgumentNullException("connectionFactory");
		}

		public async Task<Profile> GetProfileAsync(long userId)
		{
			if (userId < 0) throw new ArgumentOutOfRangeException("userId");
			using (var connection = await _connectionFactory.GetConnectionAsync())
			{
				string sqlQuery = $"SELECT Login FROM {_accountsTableName} WHERE UserId=@UserId";
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
					$"FROM {_profileInfoEntriesTableName} p, {_profileInfoTypesTableName} t " +
					$"WHERE p.UserId=@UserId AND t.Id=p.TypeId";
				var dbProfileInfoEntries = await connection.QueryAsync<DbProfileInfoEntry>(sqlQuery, new { UserId = userId });
				var profileInfo = from dbEntry in dbProfileInfoEntries
								  select new ProfileInfoEntry
								  {
									  Type = _fromDbProfileInfoTypesMapping[dbEntry.Type],
									  Value = dbEntry.Value,
									  IsPublic = dbEntry.IsPublic
								  };
				return new Profile
				{
					Login = login,
					Avatar = null,
					ProfileInfo = profileInfo.ToList()
				};
			}
		}

		public async Task<bool> SetProfileInfoAsync(long userId, List<ProfileInfoEntry> profileInfo)
		{
			if (userId < 0) throw new ArgumentOutOfRangeException("userId");
			if (profileInfo == null) throw new ArgumentNullException("profileInfo");
			using (var connection = await _connectionFactory.GetConnectionAsync())
			using (var transaction = connection.BeginTransaction())
			{
				try
				{
					string sqlQuery = $"DELETE FROM {_profileInfoEntriesTableName} WHERE UserId=@UserId";
					await connection.ExecuteAsync(sqlQuery, new { UserId = userId }, transaction: transaction);
					if (profileInfo.Count() == 0)
					{
						sqlQuery = $"SELECT UserId FROM {_accountsTableName} WHERE UserId=@UserId";
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
												   Type = _toDbProfileInfoTypesMapping[entry.Type],
												   Value = entry.Value,
												   IsPublic = entry.IsPublic
											   };
					sqlQuery = $"INSERT INTO {_profileInfoEntriesTableName} (UserId, Value, IsPublic, TypeId) " +
						$"VALUES (@UserId, @Value, @IsPublic, (SELECT Id FROM {_profileInfoTypesTableName} WHERE Type=@Type))";
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
	}
}
