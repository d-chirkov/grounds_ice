using Dapper;
using GroundsIce.Model.Abstractions;
using GroundsIce.Model.ConnectionFactories;
using GroundsIce.Model.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model.Repositories.Tests
{
	[TestFixture]
	class DbProfileRepositoryTests
	{
		private IConnectionFactory _connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");
		private const string _accountsTableName = "Accounts";
		private const string _validLogin = "login";
		private const string _validPassword = "password";
		private const string _anotherValidLogin = "loginn";
		private const string _anotherValidPassword = "passwordd";

		private DbProfileRepository _subject;
		private DbAccountRepository _accountRepository;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");
		}

		[SetUp]
		public async Task SetUp()
		{
			_accountRepository = new DbAccountRepository(_connectionFactory);
			_subject = new DbProfileRepository(_connectionFactory);
			using (var connection = await _connectionFactory.GetConnectionAsync())
			{
				await connection.ExecuteAsync($"DELETE FROM {_accountsTableName}");
			}
		}

		[Test]
		public void Ctor_Throw_When_PassingNullConnectionFactory()
		{
			Assert.Throws<ArgumentNullException>(() => new DbProfileRepository(null));
		}

		[Test]
		public void Ctor_DoesNotThrow_When_PassingValidConnectionFactory()
		{
			Assert.DoesNotThrow(() => new DbProfileRepository(_connectionFactory));
		}

		[Test]
		public void GetProfileAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _subject.GetProfileAsync(-1));
		}

		[Test]
		public async Task GetProfileAsync_ReturnNullProfile_When_PassingNotExistingUserId()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			Profile profile = await _subject.GetProfileAsync(account.UserId + 1);
			Assert.IsNull(profile);
		}

		[Test]
		public async Task GetProfileAsync_ReturnNotNullProfile_When_PassingExistingUserId()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			Profile profile = await _subject.GetProfileAsync(account.UserId);
			Assert.IsNotNull(profile);
			Assert.AreEqual(profile.Login, _validLogin);
		}

		[Test]
		public void SetProfileInfoAsync_ThrowArgumentNullException_When_PassingNullProfileInfo()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _subject.SetProfileInfoAsync(1, null));
		}

		[Test]
		public void SetProfileInfoAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _subject.SetProfileInfoAsync(-1, new List<ProfileInfoEntry>()));
		}

		[Test]
		public void SetProfileInfoAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZeroAndNullProfileInfo()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _subject.SetProfileInfoAsync(-1, null));
		}

		[Test]
		public async Task SetProfileInfoAsync_ReturnFalse_When_PassingUserIdNotExistsWithEmptyProfileInfo()
		{
			bool updated = await _subject.SetProfileInfoAsync(1, new List<ProfileInfoEntry>());
			Assert.IsFalse(updated);
		}

		[Test]
		public async Task SetProfileInfoAsync_ReturnFalse_When_PassingUserIdNotExistsWithNotEmptyProfileInfo()
		{
			bool updated = await _subject.SetProfileInfoAsync(1, new List<ProfileInfoEntry>
			{
				new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = true }
			});
			Assert.IsFalse(updated);
		}

		[Test]
		public async Task SetProfileInfoAsync_ReturnTrue_When_PassingEmptyProfileInfo()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			bool updated = await _subject.SetProfileInfoAsync(account.UserId, new List<ProfileInfoEntry>());
			Assert.IsTrue(updated);
		}

		[Test]
		public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingEmptyProfileInfo()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			await _subject.SetProfileInfoAsync(account.UserId, new List<ProfileInfoEntry>());
			Profile profile = await _subject.GetProfileAsync(account.UserId);
			Assert.AreEqual(profile.ProfileInfo.Count(), 0);
		}

		private async Task _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(long userId, List<ProfileInfoEntry> profileInfo)
		{
			await _subject.SetProfileInfoAsync(userId, profileInfo);
			Profile profile = await _subject.GetProfileAsync(userId);
			Assert.AreEqual(profile.Login, _validLogin);
			Assert.AreEqual(profile.ProfileInfo.Count(), profileInfo.Count());
			foreach (var entry in profile.ProfileInfo)
			{
				Assert.AreEqual(profileInfo.FindAll(v => v.Equals(entry)).Count(), 1);
			}
		}

		[Test]
		public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			foreach (var typeName in Enum.GetNames(typeof(ProfileInfoType)))
			{
				var type = (ProfileInfoType)Enum.Parse(typeof(ProfileInfoType), typeName);
				await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
					account.UserId,
					new[]
					{
						new ProfileInfoEntry() { Type = type, Value = "a", IsPublic = true }
					}.ToList()
				);
				await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
					account.UserId,
					new[]
					{
						new ProfileInfoEntry() { Type = type, Value = "b", IsPublic = false }
					}.ToList()
				);
			}
		}

		[Test]
		public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingFullProfileInfo()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new List<ProfileInfoEntry>
				{
					new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = false },
					new ProfileInfoEntry { Type = ProfileInfoType.LastName, Value = "b", IsPublic = true },
					new ProfileInfoEntry { Type = ProfileInfoType.MiddleName, Value = "c", IsPublic = false },
					new ProfileInfoEntry { Type = ProfileInfoType.Location, Value = "d", IsPublic = true },
					new ProfileInfoEntry { Type = ProfileInfoType.Description, Value = "e", IsPublic = false },
				}
			);
		}

		[Test]
		public async Task SetProfileInfoAsync_NotAffectGetProfileAsync_When_PassingProfileInfoWithNullOrEmptyValue()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			await _subject.SetProfileInfoAsync(account.UserId, new List<ProfileInfoEntry> { new ProfileInfoEntry { Value = null } });
			await _subject.SetProfileInfoAsync(account.UserId, new List<ProfileInfoEntry> { new ProfileInfoEntry { Value = "" } });
			Profile profile = await _subject.GetProfileAsync(account.UserId);
			Assert.AreEqual(profile.ProfileInfo.Count(), 0);
		}

		[Test]
		public async Task SetProfileInfoAsync_NotAffectGetProfileAsync_For_OtherUsers()
		{
			Account account1 = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			Account account2 = await _accountRepository.CreateAccountAsync(_anotherValidLogin, _anotherValidPassword);
			var profileInfo1 = new List<ProfileInfoEntry>
			{
				new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = false },
				new ProfileInfoEntry { Type = ProfileInfoType.LastName, Value = "b", IsPublic = true },
				new ProfileInfoEntry { Type = ProfileInfoType.MiddleName, Value = "c", IsPublic = false },
			};
			var profileInfo2 = new List<ProfileInfoEntry>
			{
				new ProfileInfoEntry { Type = ProfileInfoType.MiddleName, Value = "c", IsPublic = false },
				new ProfileInfoEntry { Type = ProfileInfoType.Location, Value = "d", IsPublic = true },
				new ProfileInfoEntry { Type = ProfileInfoType.Description, Value = "e", IsPublic = false },
			};
			await _subject.SetProfileInfoAsync(account1.UserId, profileInfo1);
			await _subject.SetProfileInfoAsync(account2.UserId, profileInfo2);
			Profile profile1 = await _subject.GetProfileAsync(account1.UserId);
			Assert.AreEqual(profile1.Login, _validLogin);
			Assert.AreEqual(profile1.ProfileInfo.Count(), profileInfo1.Count());
			foreach (var entry in profile1.ProfileInfo)
			{
				Assert.AreEqual(profileInfo1.FindAll(v => v.Equals(entry)).Count(), 1);
			}
			Profile profile2 = await _subject.GetProfileAsync(account2.UserId);
			Assert.AreEqual(profile2.Login, _anotherValidLogin);
			Assert.AreEqual(profile2.ProfileInfo.Count(), profileInfo1.Count());
			foreach (var entry in profile2.ProfileInfo)
			{
				Assert.AreEqual(profileInfo2.FindAll(v => v.Equals(entry)).Count(), 1);
			}
			await _subject.SetProfileInfoAsync(account2.UserId, new List<ProfileInfoEntry>());
			profile1 = await _subject.GetProfileAsync(account1.UserId);
			Assert.AreEqual(profile1.Login, _validLogin);
			Assert.AreEqual(profile1.ProfileInfo.Count(), profileInfo1.Count());
			foreach (var entry in profile1.ProfileInfo)
			{
				Assert.AreEqual(profileInfo1.FindAll(v => v.Equals(entry)).Count(), 1);
			}
			profile2 = await _subject.GetProfileAsync(account2.UserId);
			Assert.AreEqual(profile2.Login, _anotherValidLogin);
			Assert.AreEqual(profile2.ProfileInfo.Count(), 0);
		}
	}
}
