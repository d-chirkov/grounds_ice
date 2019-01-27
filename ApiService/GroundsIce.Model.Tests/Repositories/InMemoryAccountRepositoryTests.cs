using GroundsIce.Model.Abstractions.Repositories;
using GroundsIce.Model.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model.Repositories.Tests
{
    using LoginPassword = InMemoryAccountRepository.LoginPassword;

    [TestFixture]
    class InMemoryAccountRepositoryTests
    {
        string _login;
        string _password;
        long _userId;
        Dictionary<long, LoginPassword> _storage;
        LoginPassword _loginPassword;
        InMemoryAccountRepository _repo;

        [SetUp]
        public void SetUp()
        {
            _login = "a";
            _password = "b";
            _userId = 1;
            InMemoryAccountRepository.Clear();
            _storage = InMemoryAccountRepository.GetStorage();
            _loginPassword = new LoginPassword { Login = _login, Password = _password };
            _repo = new InMemoryAccountRepository();
        }

        [Test]
        public async Task CreateAccountAsync_ReturnValidAccount_When_PassingValidLoginAndPassword()
        {
            Account account = await _repo.CreateAccountAsync(_login, _password);
            Assert.NotNull(account);
            Assert.IsTrue(account.UserId >= 0);
            Assert.AreEqual(_login, account.Login);
        }

        [Test]
        public async Task CreateAccountAsync_SaveAccountInStorage_When_PassingValidLoginAndPassword()
        {
            Account account = await _repo.CreateAccountAsync(_login, _password);
            _storage.ContainsValue(_loginPassword);
        }

        private void _ThrowArgumentNullException_When_PassingNullLoginOrPassword(Func<string, string, Task<Account>> func)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await func(_login, null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await func(null, _password));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await func(null, null));
        }

        [Test]
        public void CreateAccountAsync_ThrowArgumentNullException_When_PassingNullLoginOrPassword()
        {
            _ThrowArgumentNullException_When_PassingNullLoginOrPassword(_repo.CreateAccountAsync);
        }

		[Test]
		public void GetAccountAsync_ThrowArgumentNullException_When_PassedNullLoginOrPassword()
		{
			_ThrowArgumentNullException_When_PassingNullLoginOrPassword(_repo.GetAccountAsync);
		}

		private async Task _CreateAccountAsync_ReturnNullAcount_When_CreatingAccountWithSameLoginSecondTime(string secondPassword)
        {
            Account firstAccount = await _repo.CreateAccountAsync(_login, _password);
            Account secondAccount = await _repo.CreateAccountAsync(_login, secondPassword);
            Assert.IsNull(secondAccount);
        }

		[Test]
        public async Task CreateAccountAsync_ReturnNullAcount_When_CreatingAccountWithSameLoginAndSamePasswordsSecondTime()
        {
            await _CreateAccountAsync_ReturnNullAcount_When_CreatingAccountWithSameLoginSecondTime(_password);
        }

        [Test]
        public async Task CreateAccountAsync_ReturnNullAcount_When_CreatingAccountWithSameLoginAndDifferentPasswordsSecondTime()
        {
            await _CreateAccountAsync_ReturnNullAcount_When_CreatingAccountWithSameLoginSecondTime(_password + "a");
        }

        [Test]
        public async Task GetAccountAsync_ReturnAccountByLoginAndPassword_When_AccountExistsInStorage()
        {
            _storage[_userId] = _loginPassword;
            Account account = await _repo.GetAccountAsync(_loginPassword.Login, _loginPassword.Password);
            Assert.NotNull(account);
            Assert.AreEqual(account.Login, _login);
            Assert.AreEqual(account.UserId, _userId);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNullAccount_When_AccountWithLoginNotExistsInStorage()
        {
            _storage[_userId] = _loginPassword;
            Account account = await _repo.GetAccountAsync(_loginPassword.Login + "a", _loginPassword.Password);
            Assert.Null(account);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNullAccount_When_AccountWithPasswordNotExistsInStorage()
        {
            _storage[_userId] = _loginPassword;
            Account account = await _repo.GetAccountAsync(_loginPassword.Login, _loginPassword.Password + "a");
            Assert.Null(account);
        }


        [Test]
        public async Task GetAccountAsync_ReturnAccountByUserId_When_AccountWithUserIdExistsInStorage()
        {
            _storage[_userId] = _loginPassword;
            Account account = await _repo.GetAccountAsync(_userId);
            Assert.NotNull(account);
            Assert.AreEqual(account.Login, _login);
            Assert.AreEqual(account.UserId, _userId);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_AccountWithUserIdNotExistsInStorage()
        {
            _storage[_userId] = _loginPassword;
            Account account = await _repo.GetAccountAsync(_userId + 1);
            Assert.Null(account);
        }

        [Test]
        public void GetAccountAsync_ThrowArgumentOutOfRangeException_When_PassedUserIdIsLessThenZero()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _repo.GetAccountAsync(-1));
        }

        private void _ThrowArgumentNullException_When_NewValueIsNull(
            Func<long, string, Task> changeValue)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await changeValue(_userId, null));
        }

        [Test]
        public void ChangeLoginAsync_ThrowArgumentNullException_When_NewValueIsNull()
        {
			_ThrowArgumentNullException_When_NewValueIsNull(_repo.ChangeLoginAsync);
        }

		[Test]
		public void ChangePasswordAsync_ThrowArgumentNullException_When_NewValueIsNull()
		{
			_ThrowArgumentNullException_When_NewValueIsNull(_repo.ChangePasswordAsync);
		}

		private void _ThrowArgumentOutOfRangeException_When_UserIdIsLessThenZero(
			Func<long, string, Task> changeValue)
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await changeValue(-1, "a"));
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await changeValue(-1, null));
		}

		[Test]
		public void ChangeLoginAsync_ThrowArgumentOutOfRangeException_When_UserIdIsLessThenZero()
		{
			_ThrowArgumentOutOfRangeException_When_UserIdIsLessThenZero(_repo.ChangeLoginAsync);
		}

		[Test]
		public void ChangePasswordAsync_ThrowArgumentOutOfRangeException_When_UserIdIsLessThenZero()
		{
			_ThrowArgumentOutOfRangeException_When_UserIdIsLessThenZero(_repo.ChangePasswordAsync);
		}

        [Test]
        public async Task ChangeLoginAsync_ChangeLoginForUser_When_NewLoginNotExistsInStorage()
        {
            string newLogin = _login + "a";
			_storage[_userId] = _loginPassword;
			bool changed = await _repo.ChangeLoginAsync(_userId, newLogin);
			Assert.IsTrue(changed);
			Assert.AreEqual(_storage[_userId].Login, newLogin);
        }

        [Test]
        public void ChangeLoginAsync_ReturnFalseAndDoNotChangeStorage_When_NewLoginAlreadyExistsInStorage()
        {
            _storage[_userId] = _loginPassword;
            string newLogin = _login + "a";
            _storage[_userId + 1] = new LoginPassword { Login = newLogin, Password = _password };
            bool changed = false;
			Assert.DoesNotThrowAsync(async () => changed = await _repo.ChangeLoginAsync(_userId, newLogin));
            Assert.IsFalse(changed);
            Assert.AreEqual(_storage[_userId].Login, _login);
        }

        private void _ThrowUserIdNotFoundException_When_UserIdNotExistsInStorage(
            Func<long, string, Task> func, 
            string newValue)
        {
            _storage[_userId] = _loginPassword;
            Assert.ThrowsAsync<UserIdNotFoundException>(async () => await func(_userId + 1, newValue));
        }

        [Test]
        public void ChangeLoginAsync_ThrowUserIdNotFoundException_When_UserIdNotExistsInStorage()
        {
            _ThrowUserIdNotFoundException_When_UserIdNotExistsInStorage(_repo.ChangeLoginAsync, _login + "a");
        }


        [Test]
        public void ChangePasswordAsync_ThrowUserIdNotFoundException_When_UserIdNotExistsInStorage()
        {
            _ThrowUserIdNotFoundException_When_UserIdNotExistsInStorage(_repo.ChangePasswordAsync, _password + "a");
        }

        [Test]
        public void ChangePasswordAsync_ChangePasswordForUser_When_NewPasswordPassed()
        {
            string newPassword = _password + "a";
			_storage[_userId] = _loginPassword;
			Assert.DoesNotThrowAsync(async () => await _repo.ChangePasswordAsync(_userId, newPassword));
			Assert.AreEqual(_storage[_userId].Password, newPassword);
        }
    }
}
