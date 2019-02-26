namespace GroundsIce.Model.Repositories.Tests
{
    using System;
    using System.Threading.Tasks;
    using Dapper;
    using GroundsIce.Model.Abstractions;
    using GroundsIce.Model.ConnectionFactories;
    using GroundsIce.Model.Entities;
    using NUnit.Framework;
    using static GroundsIce.Model.Repositories.DbAccountRepository_OLD;

    [TestFixture]
    public class DbAccountRepositoryTests
    {
        private const string AccountsTableName = "Accounts";
        private const string ProfileInfoTableName = "ProfileInfo";
        private const string ValidLogin = "login";
        private const string ValidPassword = "password";
        private const string AnotherValidLogin = "loginn";
        private const string AnotherValidPassword = "passwordd";
        private IConnectionFactory connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");

        private DbAccountRepository_OLD repository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");
        }

        [SetUp]
        public async Task SetUp()
        {
            this.repository = new DbAccountRepository_OLD(this.connectionFactory);
            using (var connection = await this.connectionFactory.GetConnectionAsync())
            {
                await connection.ExecuteAsync($"DELETE FROM {AccountsTableName}");
            }
        }

        [Test]
        public void Ctor_ThrowArgumentNullException_When_ConnectionFactoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DbAccountRepository_OLD(null));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingValidConnectionFactory()
        {
            Assert.DoesNotThrow(() => new DbAccountRepository_OLD(this.connectionFactory));
        }

        [Test]
        public void CreateAccountAsync_ThrowArgumentNullException_WhenLoginOrPasswordIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.CreateAccountAsync(null, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.CreateAccountAsync(ValidLogin, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.CreateAccountAsync(null, ValidPassword));
        }

        [Test]
        public async Task CreateAccountAsync_ReturnCreatedAccount_When_PassingNotNullLoginAndPassword()
        {
            Account_OLD account = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            Assert.NotNull(account);
            Assert.AreEqual(ValidLogin, account.Login);
        }

        public async Task CreateAccountAsync_ReturnNull_When_PassingSecondTime(string login, string password)
        {
            Account_OLD firstAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            Account_OLD secondAccount = await this.repository.CreateAccountAsync(login, password);
            Assert.IsNull(secondAccount);
        }

        [Test]
        public async Task CreateAccountAsync_ReturnNull_When_PassingSameLoginAndPasswordSecondTime()
        {
            await this.CreateAccountAsync_ReturnNull_When_PassingSecondTime(ValidLogin, ValidPassword);
        }

        [Test]
        public async Task CreateAccountAsync_ReturnNull_When_PassingSameLoginAndAnotherPasswordSecondTime()
        {
            await this.CreateAccountAsync_ReturnNull_When_PassingSecondTime(ValidLogin, AnotherValidPassword);
        }

        public async Task CreateAccountAsync_ReturnCreatedAccount_When_PassingSecondTime(string login, string password)
        {
            Account_OLD firstAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            Account_OLD secondAccount = await this.repository.CreateAccountAsync(login, password);
            Assert.NotNull(firstAccount);
            Assert.NotNull(secondAccount);
            Assert.AreEqual(ValidLogin, firstAccount.Login);
            Assert.AreEqual(AnotherValidLogin, secondAccount.Login);
        }

        [Test]
        public async Task CreateAccountAsync_ReturnCreatedAccount_When_PassingAnotherLoginAndSamePasswordSecondTime()
        {
            await this.CreateAccountAsync_ReturnCreatedAccount_When_PassingSecondTime(AnotherValidLogin, ValidPassword);
        }

        [Test]
        public async Task CreateAccountAsync_ReturnCreatedAccount_When_PassingAnotherLoginAndPasswordSecondTime()
        {
            await this.CreateAccountAsync_ReturnCreatedAccount_When_PassingSecondTime(AnotherValidLogin, AnotherValidPassword);
        }

        [Test]
        public void GetAccountAsync_ThrowArgumentNullException_WhenPassingNullLoginOrPassword()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.GetAccountAsync(null, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.GetAccountAsync(ValidLogin, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.GetAccountAsync(null, ValidPassword));
        }

        [Test]
        public void GetAccountAsync_ThrowArgumentOutOfRangeException_WhenPassingUserIdIsLessThenZero()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.repository.GetAccountAsync(-1));
        }

        [Test]
        public async Task GetAccountAsync_ReturnValidAccountByLoginAndPassword_When_CalledAfterValidCreateAccountAsyncCalling()
        {
            Account_OLD createdAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            Account_OLD anotherCreatedAccount = await this.repository.CreateAccountAsync(AnotherValidLogin, AnotherValidPassword);
            Account_OLD requestedAccount = await this.repository.GetAccountAsync(ValidLogin, ValidPassword);
            Account_OLD anotherRequestedAccount = await this.repository.GetAccountAsync(AnotherValidLogin, AnotherValidPassword);
            Assert.AreEqual(createdAccount.UserId, requestedAccount.UserId);
            Assert.AreEqual(createdAccount.Login, requestedAccount.Login);
            Assert.AreEqual(anotherCreatedAccount.UserId, anotherRequestedAccount.UserId);
            Assert.AreEqual(anotherCreatedAccount.Login, anotherRequestedAccount.Login);
        }

        [Test]
        public async Task GetAccountAsync_ReturnValidAccountById_When_CalledAfterValidCreateAccountAsyncCalling()
        {
            Account_OLD createdAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            Account_OLD anotherCreatedAccount = await this.repository.CreateAccountAsync(AnotherValidLogin, AnotherValidPassword);
            Account_OLD requestedAccount = await this.repository.GetAccountAsync(createdAccount.UserId);
            Account_OLD anotherRequestedAccount = await this.repository.GetAccountAsync(anotherCreatedAccount.UserId);
            Assert.AreEqual(createdAccount.UserId, requestedAccount.UserId);
            Assert.AreEqual(createdAccount.Login, requestedAccount.Login);
            Assert.AreEqual(anotherCreatedAccount.UserId, anotherRequestedAccount.UserId);
            Assert.AreEqual(anotherCreatedAccount.Login, anotherRequestedAccount.Login);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingLogin_And_RepoIsEmpty()
        {
            Account_OLD sameAccount = await this.repository.GetAccountAsync(ValidLogin, ValidPassword);
            Assert.IsNull(sameAccount);
        }

        public async Task GetAccountAsync_ReturnNull_When_PassingArgs(string login, string password)
        {
            Account_OLD createdAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            Account_OLD sameAccount = await this.repository.GetAccountAsync(login, password);
            Assert.IsNull(sameAccount);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingLogin_And_RepoIsNotEmpty()
        {
            await this.GetAccountAsync_ReturnNull_When_PassingArgs(AnotherValidLogin, ValidPassword);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingPasswordNotSuitableForLogin_And_RepoIsNotEmpty()
        {
            await this.GetAccountAsync_ReturnNull_When_PassingArgs(ValidLogin, AnotherValidPassword);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingLoginAndPassword_And_RepoIsNotEmpty()
        {
            await this.GetAccountAsync_ReturnNull_When_PassingArgs(AnotherValidPassword, AnotherValidPassword);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingUserId_And_RepoIsNotEmpty()
        {
            Account_OLD createdAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            Account_OLD sameAccount = await this.repository.GetAccountAsync(createdAccount.UserId + 1);
            Assert.IsNull(sameAccount);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingUserId_And_RepoIsEmpty()
        {
            Account_OLD sameAccount = await this.repository.GetAccountAsync(1);
            Assert.IsNull(sameAccount);
        }

        [Test]
        public void ChangeLoginAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.repository.ChangeLoginAsync(-1, ValidLogin));
        }

        [Test]
        public void ChangePasswordAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.repository.ChangePasswordAsync(-1, ValidPassword, AnotherValidPassword));
        }

        [Test]
        public void ChangeLoginAsync_ThrowArgumentNullException_When_PassingNullLogin()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.ChangeLoginAsync(1, null));
        }

        [Test]
        public void ChangePasswordAsync_ThrowArgumentNullException_When_PassingNullOldOrNewPassword()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.ChangePasswordAsync(1, ValidPassword, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.ChangePasswordAsync(1, null, ValidPassword));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.ChangePasswordAsync(1, null, null));
        }

        [Test]
        public async Task ChangeLoginAsync_ThrowDbAccountRepositoryException_When_PassingNotExistingUserId_And_RepoIsNotEmpty()
        {
            Account_OLD createdAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            Assert.ThrowsAsync<DbAccountRepositoryException>(
                () => this.repository.ChangeLoginAsync(createdAccount.UserId + 1, AnotherValidLogin));
            Account_OLD sameAccount = await this.repository.GetAccountAsync(ValidLogin, ValidPassword);
            Assert.NotNull(sameAccount);
            Assert.AreEqual(createdAccount.UserId, sameAccount.UserId);
        }

        [Test]
        public async Task ChangePasswordAsync_ReturnFalse_When_PassingNotExistingUserId_And_RepoIsNotEmpty()
        {
            Account_OLD createdAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            bool changed = await this.repository.ChangePasswordAsync(createdAccount.UserId + 1, ValidPassword, AnotherValidPassword);
            Assert.IsFalse(changed);
            Account_OLD sameAccount = await this.repository.GetAccountAsync(ValidLogin, ValidPassword);
            Assert.NotNull(sameAccount);
            Assert.AreEqual(createdAccount.UserId, sameAccount.UserId);
        }

        [Test]
        public async Task ChangePasswordAsync_ReturnFalse_When_PassingNotExistingUserId_And_RepoIsEmpty()
        {
            bool changed = await this.repository.ChangePasswordAsync(1, ValidPassword, AnotherValidPassword);
            Assert.IsFalse(changed);
        }

        [Test]
        public async Task ChangeLoginAsync_ReturnTrue_When_PassingValidUserIdAndLogin()
        {
            Account_OLD createdAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            bool changed = await this.repository.ChangeLoginAsync(createdAccount.UserId, AnotherValidLogin);
            Assert.IsTrue(changed);
        }

        [Test]
        public async Task ChangePasswordAsync_ReturnTrue_When_PassingValidUserIdAndLogin()
        {
            Account_OLD createdAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            bool changed = await this.repository.ChangePasswordAsync(createdAccount.UserId, ValidPassword, AnotherValidPassword);
            Assert.IsTrue(changed);
        }

        [Test]
        public async Task ChangeLoginAsync_PerformChangingForGetAccountAsyncResult_When_PassingValidUserIdAndLogin()
        {
            Account_OLD createdAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            bool changed = await this.repository.ChangeLoginAsync(createdAccount.UserId, AnotherValidLogin);
            Account_OLD changedAccount = await this.repository.GetAccountAsync(createdAccount.UserId);
            Assert.AreEqual(changedAccount.Login, AnotherValidLogin);
            Account_OLD sameChangedAccount = await this.repository.GetAccountAsync(AnotherValidLogin, ValidPassword);
            Assert.AreEqual(sameChangedAccount.UserId, createdAccount.UserId);
            Account_OLD notValidAccount = await this.repository.GetAccountAsync(ValidLogin, ValidPassword);
            Assert.IsNull(notValidAccount);
        }

        [Test]
        public async Task ChangePasswordAsync_PerformChangingForGetAccountAsyncResult_When_PassingValidUserIdAndLogin()
        {
            Account_OLD createdAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            bool changed = await this.repository.ChangePasswordAsync(createdAccount.UserId, ValidPassword, AnotherValidPassword);
            Account_OLD changedAccount = await this.repository.GetAccountAsync(ValidLogin, AnotherValidPassword);
            Assert.AreEqual(changedAccount.UserId, createdAccount.UserId);
            Account_OLD notValidAccount = await this.repository.GetAccountAsync(ValidLogin, ValidPassword);
            Assert.IsNull(notValidAccount);
        }

        [Test]
        public async Task ChangePasswordAsync_ReturnFalse_When_PassingNotValidOldPassword()
        {
            Account_OLD createdAccount = await this.repository.CreateAccountAsync(ValidLogin, ValidPassword);
            bool changed = await this.repository.ChangePasswordAsync(createdAccount.UserId, ValidPassword + "a", AnotherValidPassword);
            Assert.IsFalse(changed);
            Account_OLD sameAccount = await this.repository.GetAccountAsync(ValidLogin, ValidPassword);
            Assert.NotNull(sameAccount);
            Assert.AreEqual(createdAccount.UserId, sameAccount.UserId);
        }
    }
}
