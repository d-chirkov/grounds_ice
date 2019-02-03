using System;
using System.Collections.Generic;
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
		[Table("ProfileInfo")]
		class DbProfileInfo
		{
			[Key]
			public long UserId { get; set; }
			public string FirstName { get; set; }
			public bool IsFirstNamePublic { get; set; }
			public string MiddleName { get; set; }
			public bool IsMiddleNamePublic { get; set; }
			public string Surname { get; set; }
			public bool IsSurnamePublic { get; set; }
			public string Location { get; set; }
			public bool IsLocationPublic { get; set; }
			public string Description { get; set; }
			public bool IsDescriptionPublic { get; set; }
		}

		class DbProfile : DbProfileInfo
		{
			public string Login { get; set; }
		}

		private IConnectionFactory _connectionFactory;
		private const string _profileInfoTableName = "ProfileInfo";
		private const string _accountsTableName = "Accounts";

		public DbProfileRepository(IConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory ?? throw new ArgumentNullException("connectionFactory");
		}

		public async Task<Profile> GetProfileAsync(long userId)
		{
			//TODO: change request for avatar
			if (userId < 0) throw new ArgumentOutOfRangeException("userId");
			string sqlQuery = $"SELECT a.Login, p.* FROM {_accountsTableName} a, {_profileInfoTableName} p WHERE a.UserId=p.UserId AND p.UserId=@UserId";
			using (var connection = await _connectionFactory.GetConnectionAsync())
			{
				var dbResult = await connection.QueryAsync<DbProfile>(sqlQuery, new { UserId = userId });
				DbProfile dbProfile =
					dbResult.Count() == 0 ? null :
					dbResult.Count() > 1 ? throw new DbProfileRepositoryException("Multiple profile infos were found by user id") :
					dbResult.FirstOrDefault() ?? throw new DbProfileRepositoryException("Null profile info was returned");
				return dbProfile == null ? null : new Profile()
				{
					Login = dbProfile.Login,
					ProfileInfo = new ProfileInfo()
					{
						FirstName = CreateProfileInfoEntryFrom(dbProfile.FirstName, dbProfile.IsFirstNamePublic),
						MiddleName = CreateProfileInfoEntryFrom(dbProfile.MiddleName, dbProfile.IsMiddleNamePublic),
						Surname = CreateProfileInfoEntryFrom(dbProfile.Surname, dbProfile.IsSurnamePublic),
						Location = CreateProfileInfoEntryFrom(dbProfile.Location, dbProfile.IsLocationPublic),
						Description = CreateProfileInfoEntryFrom(dbProfile.Description, dbProfile.IsDescriptionPublic)
					},
					Avatar = null
				};
			}
		}

		private ProfileInfoEntry CreateProfileInfoEntryFrom(string value, bool isPublic)
		{
			return value != null && value != "" ? new ProfileInfoEntry() { Value = value, IsPublic = isPublic } : null;
		}

		public async Task<bool> SetProfileInfoAsync(long userId, ProfileInfo profileInfo)
		{
			if (userId < 0) throw new ArgumentOutOfRangeException("userId");
			if (profileInfo == null) throw new ArgumentNullException("profileInfo");
			using (var connection = await _connectionFactory.GetConnectionAsync())
			{
				var dbProfileInfo = new DbProfileInfo
				{
					UserId = userId,
					FirstName = GetValueFrom(profileInfo.FirstName),
					IsFirstNamePublic = GetIsPublicFlagFrom(profileInfo.FirstName),
					MiddleName = GetValueFrom(profileInfo.MiddleName),
					IsMiddleNamePublic = GetIsPublicFlagFrom(profileInfo.MiddleName),
					Surname = GetValueFrom(profileInfo.Surname),
					IsSurnamePublic = GetIsPublicFlagFrom(profileInfo.Surname),
					Location = GetValueFrom(profileInfo.Location),
					IsLocationPublic = GetIsPublicFlagFrom(profileInfo.Location),
					Description = GetValueFrom(profileInfo.Description),
					IsDescriptionPublic = GetIsPublicFlagFrom(profileInfo.Description),
				};
				return await connection.UpdateAsync(dbProfileInfo);
			}
		}

		private string GetValueFrom(ProfileInfoEntry entry)
		{
			return entry == null || entry.Value == null || entry.Value == "" ? null : entry.Value;
		}

		private bool GetIsPublicFlagFrom(ProfileInfoEntry entry)
		{
			return entry == null || entry.Value == null || entry.Value == "" ? false : entry.IsPublic;
		}
	}
}
