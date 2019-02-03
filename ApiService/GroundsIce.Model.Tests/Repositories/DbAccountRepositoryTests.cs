using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroundsIce.Model.Abstractions;
using GroundsIce.Model.ConnectionFactories;
using Dapper.Contrib.Extensions;
using Dapper;
using GroundsIce.Model.Entities;

namespace GroundsIce.Model.Repositories.Tests
{
	[TestFixture]
	class DbAccountRepositoryTests
	{
		private IConnectionFactory _connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");
		private const string _accountsTableName = "Accounts";
		private const string _profileInfoTableName = "ProfileInfo";
		private const string _validLogin = "login";
		private const string _validPassword = "password";
		private const string _anotherValidLogin = "loginn";
		private const string _anotherValidPassword = "passwordd";

		private DbAccountRepository _repository;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");
		}

		[SetUp]
		public async Task SetUp()
		{
			_repository = new DbAccountRepository(_connectionFactory);
			using (var connection = await _connectionFactory.GetConnectionAsync())
			{
				await connection.ExecuteAsync($"DELETE FROM {_accountsTableName}");
			}
		}

		[Test]
		public void Ctor_ThrowArgumentNullException_When_ConnectionFactoryIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => new DbAccountRepository(null));
		}

		[Test]
		public void Ctor_DoesNotThrow_When_PassingValidConnectionFactory()
		{
			Assert.DoesNotThrow(() => new DbAccountRepository(_connectionFactory));
		}

		[Test]
		public void CreateAccountAsync_ThrowArgumentNullException_WhenLoginOrPasswordIsNull()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _repository.CreateAccountAsync(null, null));
			Assert.ThrowsAsync<ArgumentNullException>(() => _repository.CreateAccountAsync(_validLogin, null));
			Assert.ThrowsAsync<ArgumentNullException>(() => _repository.CreateAccountAsync(null, _validPassword));
		}

		[Test]
		public async Task CreateAccountAsync_ReturnCreatedAccount_When_PassingNotNullLoginAndPassword()
		{
			Account account = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			Assert.NotNull(account);
			Assert.AreEqual(_validLogin, account.Login);
		}

		private async Task _CreateAccountAsync_ReturnNull_When_PassingSecondTime(string login, string password)
		{
			Account firstAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			Account secondAccount = await _repository.CreateAccountAsync(login, password);
			Assert.IsNull(secondAccount);
		}

		[Test]
		public async Task CreateAccountAsync_ReturnNull_When_PassingSameLoginAndPasswordSecondTime()
		{
			await _CreateAccountAsync_ReturnNull_When_PassingSecondTime(_validLogin, _validPassword);
		}

		[Test]
		public async Task CreateAccountAsync_ReturnNull_When_PassingSameLoginAndAnotherPasswordSecondTime()
		{
			await _CreateAccountAsync_ReturnNull_When_PassingSecondTime(_validLogin, _anotherValidPassword);
		}

		private async Task _CreateAccountAsync_ReturnCreatedAccount_When_PassingSecondTime(string login, string password)
		{
			Account firstAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			Account secondAccount = await _repository.CreateAccountAsync(login, password);
			Assert.NotNull(firstAccount);
			Assert.NotNull(secondAccount);
			Assert.AreEqual(_validLogin, firstAccount.Login);
			Assert.AreEqual(_anotherValidLogin, secondAccount.Login);
		}

		[Test]
		public async Task CreateAccountAsync_ReturnCreatedAccount_When_PassingAnotherLoginAndSamePasswordSecondTime()
		{
			await _CreateAccountAsync_ReturnCreatedAccount_When_PassingSecondTime(_anotherValidLogin, _validPassword);
		}

		[Test]
		public async Task CreateAccountAsync_ReturnCreatedAccount_When_PassingAnotherLoginAndPasswordSecondTime()
		{
			await _CreateAccountAsync_ReturnCreatedAccount_When_PassingSecondTime(_anotherValidLogin, _anotherValidPassword);
		}

		[Test]
		public void GetAccountAsync_ThrowArgumentNullException_WhenPassingNullLoginOrPassword()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _repository.GetAccountAsync(null, null));
			Assert.ThrowsAsync<ArgumentNullException>(() => _repository.GetAccountAsync(_validLogin, null));
			Assert.ThrowsAsync<ArgumentNullException>(() => _repository.GetAccountAsync(null, _validPassword));
		}

		[Test]
		public void GetAccountAsync_ThrowArgumentOutOfRangeException_WhenPassingUserIdIsLessThenZero()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _repository.GetAccountAsync(-1));
		}

		[Test]
		public async Task GetAccountAsync_ReturnValidAccountByLoginAndPassword_When_CalledAfterValidCreateAccountAsyncCalling()
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			Account anotherCreatedAccount = await _repository.CreateAccountAsync(_anotherValidLogin, _anotherValidPassword);
			Account requestedAccount = await _repository.GetAccountAsync(_validLogin, _validPassword);
			Account anotherRequestedAccount = await _repository.GetAccountAsync(_anotherValidLogin, _anotherValidPassword);
			Assert.AreEqual(createdAccount.UserId, requestedAccount.UserId);
			Assert.AreEqual(createdAccount.Login, requestedAccount.Login);
			Assert.AreEqual(anotherCreatedAccount.UserId, anotherRequestedAccount.UserId);
			Assert.AreEqual(anotherCreatedAccount.Login, anotherRequestedAccount.Login);
		}

		[Test]
		public async Task GetAccountAsync_ReturnValidAccountById_When_CalledAfterValidCreateAccountAsyncCalling()
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			Account anotherCreatedAccount = await _repository.CreateAccountAsync(_anotherValidLogin, _anotherValidPassword);
			Account requestedAccount = await _repository.GetAccountAsync(createdAccount.UserId);
			Account anotherRequestedAccount = await _repository.GetAccountAsync(anotherCreatedAccount.UserId);
			Assert.AreEqual(createdAccount.UserId, requestedAccount.UserId);
			Assert.AreEqual(createdAccount.Login, requestedAccount.Login);
			Assert.AreEqual(anotherCreatedAccount.UserId, anotherRequestedAccount.UserId);
			Assert.AreEqual(anotherCreatedAccount.Login, anotherRequestedAccount.Login);
		}

		[Test]
		public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingLogin_And_RepoIsEmpty()
		{
			Account sameAccount = await _repository.GetAccountAsync(_validLogin, _validPassword);
			Assert.IsNull(sameAccount);
		}

		private async Task _GetAccountAsync_ReturnNull_When_PassingArgs(string login, string password)
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			Account sameAccount = await _repository.GetAccountAsync(login, password);
			Assert.IsNull(sameAccount);
		}

		[Test]
		public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingLogin_And_RepoIsNotEmpty()
		{
			await _GetAccountAsync_ReturnNull_When_PassingArgs(_anotherValidLogin, _validPassword);
		}

		[Test]
		public async Task GetAccountAsync_ReturnNull_When_PassingPasswordNotSuitableForLogin_And_RepoIsNotEmpty()
		{
			await _GetAccountAsync_ReturnNull_When_PassingArgs(_validLogin, _anotherValidPassword);
		}

		[Test]
		public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingLoginAndPassword_And_RepoIsNotEmpty()
		{
			await _GetAccountAsync_ReturnNull_When_PassingArgs(_anotherValidPassword, _anotherValidPassword);
		}

		[Test]
		public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingUserId_And_RepoIsNotEmpty()
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			Account sameAccount = await _repository.GetAccountAsync(createdAccount.UserId + 1);
			Assert.IsNull(sameAccount);
		}

		[Test]
		public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingUserId_And_RepoIsEmpty()
		{
			Account sameAccount = await _repository.GetAccountAsync(1);
			Assert.IsNull(sameAccount);
		}

		[Test]
		public void ChangeLoginAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _repository.ChangeLoginAsync(-1, _validLogin));
		}

		[Test]
		public void ChangePasswordAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _repository.ChangePasswordAsync(-1, _validPassword, _anotherValidPassword));
		}

		[Test]
		public void ChangeLoginAsync_ThrowArgumentNullException_When_PassingNullLogin()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _repository.ChangeLoginAsync(1, null));
		}

		[Test]
		public void ChangePasswordAsync_ThrowArgumentNullException_When_PassingNullOldOrNewPassword()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _repository.ChangePasswordAsync(1, _validPassword, null));
			Assert.ThrowsAsync<ArgumentNullException>(() => _repository.ChangePasswordAsync(1, null, _validPassword));
			Assert.ThrowsAsync<ArgumentNullException>(() => _repository.ChangePasswordAsync(1, null, null));
		}

		[Test]
		public async Task ChangeLoginAsync_ThrowDbAccountRepositoryException_When_PassingNotExistingUserId_And_RepoIsNotEmpty()
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			Assert.ThrowsAsync<DbAccountRepositoryException>(
				() => _repository.ChangeLoginAsync(createdAccount.UserId + 1, _anotherValidLogin));
			Account sameAccount = await _repository.GetAccountAsync(_validLogin, _validPassword);
			Assert.NotNull(sameAccount);
			Assert.AreEqual(createdAccount.UserId, sameAccount.UserId);
		}

		[Test]
		public async Task ChangePasswordAsync_ReturnFalse_When_PassingNotExistingUserId_And_RepoIsNotEmpty()
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			bool changed = await _repository.ChangePasswordAsync(createdAccount.UserId + 1, _validPassword, _anotherValidPassword);
			Assert.IsFalse(changed);
			Account sameAccount = await _repository.GetAccountAsync(_validLogin, _validPassword);
			Assert.NotNull(sameAccount);
			Assert.AreEqual(createdAccount.UserId, sameAccount.UserId);
		}

		[Test]
		public async Task ChangePasswordAsync_ReturnFalse_When_PassingNotExistingUserId_And_RepoIsEmpty()
		{
			bool changed = await _repository.ChangePasswordAsync(1, _validPassword, _anotherValidPassword);
			Assert.IsFalse(changed);
		}

		[Test]
		public async Task ChangeLoginAsync_ReturnTrue_When_PassingValidUserIdAndLogin()
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			bool changed = await _repository.ChangeLoginAsync(createdAccount.UserId, _anotherValidLogin);
			Assert.IsTrue(changed);
		}

		[Test]
		public async Task ChangePasswordAsync_ReturnTrue_When_PassingValidUserIdAndLogin()
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			bool changed = await _repository.ChangePasswordAsync(createdAccount.UserId, _validPassword, _anotherValidPassword);
			Assert.IsTrue(changed);
		}

		[Test]
		public async Task ChangeLoginAsync_PerformChangingForGetAccountAsyncResult_When_PassingValidUserIdAndLogin()
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			bool changed = await _repository.ChangeLoginAsync(createdAccount.UserId, _anotherValidLogin);
			Account changedAccount = await _repository.GetAccountAsync(createdAccount.UserId);
			Assert.AreEqual(changedAccount.Login, _anotherValidLogin);
			Account sameChangedAccount = await _repository.GetAccountAsync(_anotherValidLogin, _validPassword);
			Assert.AreEqual(sameChangedAccount.UserId, createdAccount.UserId);
			Account notValidAccount = await _repository.GetAccountAsync(_validLogin, _validPassword);
			Assert.IsNull(notValidAccount);
		}
		
		[Test]
		public async Task ChangePasswordAsync_PerformChangingForGetAccountAsyncResult_When_PassingValidUserIdAndLogin()
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			bool changed = await _repository.ChangePasswordAsync(createdAccount.UserId, _validPassword, _anotherValidPassword);
			Account changedAccount = await _repository.GetAccountAsync(_validLogin, _anotherValidPassword);
			Assert.AreEqual(changedAccount.UserId, createdAccount.UserId);
			Account notValidAccount = await _repository.GetAccountAsync(_validLogin, _validPassword);
			Assert.IsNull(notValidAccount);
		}

		[Test]
		public async Task ChangePasswordAsync_ReturnFalse_When_PassingNotValidOldPassword()
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			bool changed = await _repository.ChangePasswordAsync(createdAccount.UserId, _validPassword + "a", _anotherValidPassword);
			Assert.IsFalse(changed);
			Account sameAccount = await _repository.GetAccountAsync(_validLogin, _validPassword);
			Assert.NotNull(sameAccount);
			Assert.AreEqual(createdAccount.UserId, sameAccount.UserId);
		}
	}
}
