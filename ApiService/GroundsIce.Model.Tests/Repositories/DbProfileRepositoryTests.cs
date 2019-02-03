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
		private const string _profileInfoTableName = "ProfileInfo";
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
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _subject.SetProfileInfoAsync(-1, new ProfileInfo()));
		}

		[Test]
		public void SetProfileInfoAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZeroAndNullProfileInfo()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _subject.SetProfileInfoAsync(-1, null));
		}

		[Test]
		public async Task SetProfileInfoAsync_ReturnFalse_When_PassingUserIdNotExists()
		{
			bool updated = await _subject.SetProfileInfoAsync(1, new ProfileInfo());
			Assert.IsFalse(updated);
		}

		[Test]
		public async Task SetProfileInfoAsync_ReturnTrue_When_PassingEmptyProfileInfo()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			bool updated = await _subject.SetProfileInfoAsync(account.UserId, new ProfileInfo());
			Assert.IsTrue(updated);
		}

		[Test]
		public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingEmptyProfileInfo()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			await _subject.SetProfileInfoAsync(account.UserId, new ProfileInfo());
			Profile profile = await _subject.GetProfileAsync(account.UserId);
			Assert.IsNull(profile.ProfileInfo.FirstName);
			Assert.IsNull(profile.ProfileInfo.MiddleName);
			Assert.IsNull(profile.ProfileInfo.Surname);
			Assert.IsNull(profile.ProfileInfo.Location);
			Assert.IsNull(profile.ProfileInfo.Description);
		}

		private async Task _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(long userId, ProfileInfo profileInfo)
		{
			await _subject.SetProfileInfoAsync(userId, profileInfo);
			Profile profile = await _subject.GetProfileAsync(userId);
			Assert.AreEqual(profile.Login, _validLogin);
			Assert.AreEqual(profile.ProfileInfo.FirstName, profileInfo.FirstName);
			Assert.AreEqual(profile.ProfileInfo.MiddleName, profileInfo.MiddleName);
			Assert.AreEqual(profile.ProfileInfo.Surname, profileInfo.Surname);
			Assert.AreEqual(profile.ProfileInfo.Location, profileInfo.Location);
			Assert.AreEqual(profile.ProfileInfo.Description, profileInfo.Description);
		}

		[Test]
		public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfoWithFirstName()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new ProfileInfo(){
					FirstName = new ProfileInfoEntry() { Value = "a", IsPublic = true }
				}
			);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new ProfileInfo()
				{
					FirstName = new ProfileInfoEntry() { Value = "a", IsPublic = false }
				}
			);
		}

		[Test]
		public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfoWithMiddleName()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new ProfileInfo()
				{
					MiddleName = new ProfileInfoEntry() { Value = "a", IsPublic = true }
				}
			);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new ProfileInfo()
				{
					MiddleName = new ProfileInfoEntry() { Value = "a", IsPublic = false }
				}
			);
		}

		[Test]
		public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfoWithSurname()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new ProfileInfo()
				{
					Surname = new ProfileInfoEntry() { Value = "a", IsPublic = true }
				}
			);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new ProfileInfo()
				{
					Surname = new ProfileInfoEntry() { Value = "a", IsPublic = false }
				}
			);
		}

		[Test]
		public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfoWithLocation()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new ProfileInfo()
				{
					Location = new ProfileInfoEntry() { Value = "a", IsPublic = true }
				}
			);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new ProfileInfo()
				{
					Location = new ProfileInfoEntry() { Value = "a", IsPublic = false }
				}
			);
		}

		[Test]
		public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfoWithDescription()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new ProfileInfo()
				{
					Description = new ProfileInfoEntry() { Value = "a", IsPublic = true }
				}
			);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new ProfileInfo()
				{
					Description = new ProfileInfoEntry() { Value = "a", IsPublic = false }
				}
			);
		}

		[Test]
		public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingFullProfileInfo()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			await _SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
				account.UserId,
				new ProfileInfo()
				{
					FirstName = new ProfileInfoEntry() { Value = "a", IsPublic = true },
					MiddleName = new ProfileInfoEntry() { Value = "b", IsPublic = false },
					Surname = new ProfileInfoEntry() { Value = "c", IsPublic = true },
					Location = new ProfileInfoEntry() { Value = "d", IsPublic = false },
					Description = new ProfileInfoEntry() { Value = "e", IsPublic = true }
				}
			);
		}

		[Test]
		public async Task SetProfileInfoAsync_NotAffectGetProfileAsync_When_PassingProfileInfoWithNullOrEmptyValue()
		{
			Account account = await _accountRepository.CreateAccountAsync(_validLogin, _validPassword);
			await _subject.SetProfileInfoAsync(account.UserId, new ProfileInfo() { FirstName = null } );
			await _subject.SetProfileInfoAsync(account.UserId, new ProfileInfo() { FirstName = new ProfileInfoEntry() { Value = null, IsPublic = true } });
			await _subject.SetProfileInfoAsync(account.UserId, new ProfileInfo() { FirstName = new ProfileInfoEntry() { Value = "", IsPublic = false } });
			Profile profile = await _subject.GetProfileAsync(account.UserId);
			Assert.IsNull(profile.ProfileInfo.FirstName);
			
		}
	}
}
