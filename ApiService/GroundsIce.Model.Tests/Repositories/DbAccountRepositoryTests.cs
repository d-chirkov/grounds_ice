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
		private const string _tableName = "Accounts";
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
				await connection.ExecuteAsync($"DELETE FROM {_tableName}");
			}
		}

		[Test]
		public void Ctor_Throw_When_ConnectionFactoryIsNull()
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

		private async Task _GetAccountAsync_ReturnNull_When_PassingArgs(string login, string password)
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			Account sameAccount = await _repository.GetAccountAsync(login, password);
			Assert.IsNull(sameAccount);
		}

		[Test]
		public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingLogin()
		{
			await _GetAccountAsync_ReturnNull_When_PassingArgs(_anotherValidLogin, _validPassword);
		}

		[Test]
		public async Task GetAccountAsync_ReturnNull_When_PassingPasswordNotSuitableForLogin()
		{
			await _GetAccountAsync_ReturnNull_When_PassingArgs(_validLogin, _anotherValidPassword);
		}

		[Test]
		public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingLoginAndPassword()
		{
			await _GetAccountAsync_ReturnNull_When_PassingArgs(_anotherValidPassword, _anotherValidPassword);
		}

		[Test]
		public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingUserId()
		{
			Account createdAccount = await _repository.CreateAccountAsync(_validLogin, _validPassword);
			Account sameAccount = await _repository.GetAccountAsync(createdAccount.UserId + 1);
			Assert.IsNull(sameAccount);
		}
	}
}
