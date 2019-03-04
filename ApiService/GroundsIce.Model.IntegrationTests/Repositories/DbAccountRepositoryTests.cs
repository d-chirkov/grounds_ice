namespace GroundsIce.Model.Repositories.IntegrationTests
{
    using System;
    using System.Threading.Tasks;
    using Dapper;
    using GroundsIce.Model.Abstractions;
    using GroundsIce.Model.ConnectionFactories;
    using GroundsIce.Model.Entities;
    using NUnit.Framework;
    using static GroundsIce.Model.Repositories.DbAccountRepository;

    [TestFixture]
    public class DbAccountRepositoryTests
    {
        private const string AccountsTableName = "Accounts";
        private const string ProfileInfoTableName = "ProfileInfo";
        private readonly Login validLogin = new Login("login");
        private readonly Password validPassword = new Password("password");
        private readonly Login anotherValidLogin = new Login("loginn");
        private readonly Password anotherValidPassword = new Password("passwordd");
        private IConnectionFactory connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");

        private DbAccountRepository repository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");
        }

        [SetUp]
        public async Task SetUp()
        {
            this.repository = new DbAccountRepository(this.connectionFactory);
            using (var connection = await this.connectionFactory.GetConnectionAsync())
            {
                await connection.ExecuteAsync($"DELETE FROM {AccountsTableName}");
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
            Assert.DoesNotThrow(() => new DbAccountRepository(this.connectionFactory));
        }

        [Test]
        public void CreateAccountAsync_ThrowArgumentNullException_WhenLoginOrPasswordIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.CreateAccountAsync(null, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.CreateAccountAsync(this.validLogin, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.CreateAccountAsync(null, this.validPassword));
        }

        [Test]
        public async Task CreateAccountAsync_ReturnCreatedAccount_When_PassingNotNullLoginAndPassword()
        {
            Account account = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            Assert.NotNull(account);
            Assert.AreEqual(this.validLogin, account.Login);
        }

        public async Task CreateAccountAsync_ReturnNull_When_PassingSecondTime(Login login, Password password)
        {
            Account firstAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            Account secondAccount = await this.repository.CreateAccountAsync(login, password);
            Assert.IsNull(secondAccount);
        }

        [Test]
        public async Task CreateAccountAsync_ReturnNull_When_PassingSameLoginAndPasswordSecondTime()
        {
            await this.CreateAccountAsync_ReturnNull_When_PassingSecondTime(this.validLogin, this.validPassword);
        }

        [Test]
        public async Task CreateAccountAsync_ReturnNull_When_PassingSameLoginAndAnotherPasswordSecondTime()
        {
            await this.CreateAccountAsync_ReturnNull_When_PassingSecondTime(this.validLogin, this.anotherValidPassword);
        }

        public async Task CreateAccountAsync_ReturnCreatedAccount_When_PassingSecondTime(Login login, Password password)
        {
            Account firstAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            Account secondAccount = await this.repository.CreateAccountAsync(login, password);
            Assert.NotNull(firstAccount);
            Assert.NotNull(secondAccount);
            Assert.AreEqual(this.validLogin, firstAccount.Login);
            Assert.AreEqual(this.anotherValidLogin, secondAccount.Login);
        }

        [Test]
        public async Task CreateAccountAsync_ReturnCreatedAccount_When_PassingAnotherLoginAndSamePasswordSecondTime()
        {
            await this.CreateAccountAsync_ReturnCreatedAccount_When_PassingSecondTime(this.anotherValidLogin, this.validPassword);
        }

        [Test]
        public async Task CreateAccountAsync_ReturnCreatedAccount_When_PassingAnotherLoginAndPasswordSecondTime()
        {
            await this.CreateAccountAsync_ReturnCreatedAccount_When_PassingSecondTime(this.anotherValidLogin, this.anotherValidPassword);
        }

        [Test]
        public void GetAccountAsync_ThrowArgumentNullException_WhenPassingNullLoginOrPassword()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.GetAccountAsync(null, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.GetAccountAsync(this.validLogin, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.GetAccountAsync(null, this.validPassword));
        }

        [Test]
        public void GetAccountAsync_ThrowArgumentOutOfRangeException_WhenPassingUserIdIsLessThenZero()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.repository.GetAccountAsync(-1));
        }

        [Test]
        public async Task GetAccountAsync_ReturnValidAccountByLoginAndPassword_When_CalledAfterValidCreateAccountAsyncCalling()
        {
            Account createdAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            Account anotherCreatedAccount = await this.repository.CreateAccountAsync(this.anotherValidLogin, this.anotherValidPassword);
            Account requestedAccount = await this.repository.GetAccountAsync(this.validLogin, this.validPassword);
            Account anotherRequestedAccount = await this.repository.GetAccountAsync(this.anotherValidLogin, this.anotherValidPassword);
            Assert.AreEqual(createdAccount.UserId, requestedAccount.UserId);
            Assert.AreEqual(createdAccount.Login, requestedAccount.Login);
            Assert.AreEqual(anotherCreatedAccount.UserId, anotherRequestedAccount.UserId);
            Assert.AreEqual(anotherCreatedAccount.Login, anotherRequestedAccount.Login);
        }

        [Test]
        public async Task GetAccountAsync_ReturnValidAccountById_When_CalledAfterValidCreateAccountAsyncCalling()
        {
            Account createdAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            Account anotherCreatedAccount = await this.repository.CreateAccountAsync(this.anotherValidLogin, this.anotherValidPassword);
            Account requestedAccount = await this.repository.GetAccountAsync(createdAccount.UserId);
            Account anotherRequestedAccount = await this.repository.GetAccountAsync(anotherCreatedAccount.UserId);
            Assert.AreEqual(createdAccount.UserId, requestedAccount.UserId);
            Assert.AreEqual(createdAccount.Login, requestedAccount.Login);
            Assert.AreEqual(anotherCreatedAccount.UserId, anotherRequestedAccount.UserId);
            Assert.AreEqual(anotherCreatedAccount.Login, anotherRequestedAccount.Login);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingLogin_And_RepoIsEmpty()
        {
            Account sameAccount = await this.repository.GetAccountAsync(this.validLogin, this.validPassword);
            Assert.IsNull(sameAccount);
        }

        public async Task GetAccountAsync_ReturnNull_When_PassingArgs(Login login, Password password)
        {
            Account createdAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            Account sameAccount = await this.repository.GetAccountAsync(login, password);
            Assert.IsNull(sameAccount);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingLogin_And_RepoIsNotEmpty()
        {
            await this.GetAccountAsync_ReturnNull_When_PassingArgs(this.anotherValidLogin, this.validPassword);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingPasswordNotSuitableForLogin_And_RepoIsNotEmpty()
        {
            await this.GetAccountAsync_ReturnNull_When_PassingArgs(this.validLogin, this.anotherValidPassword);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingLoginAndPassword_And_RepoIsNotEmpty()
        {
            await this.GetAccountAsync_ReturnNull_When_PassingArgs(this.anotherValidLogin, this.anotherValidPassword);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingUserId_And_RepoIsNotEmpty()
        {
            Account createdAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            Account sameAccount = await this.repository.GetAccountAsync(createdAccount.UserId + 1);
            Assert.IsNull(sameAccount);
        }

        [Test]
        public async Task GetAccountAsync_ReturnNull_When_PassingNotExistingUserId_And_RepoIsEmpty()
        {
            Account sameAccount = await this.repository.GetAccountAsync(1);
            Assert.IsNull(sameAccount);
        }

        [Test]
        public void ChangeLoginAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.repository.ChangeLoginAsync(-1, this.validLogin));
        }

        [Test]
        public void ChangePasswordAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.repository.ChangePasswordAsync(-1, this.validPassword, this.anotherValidPassword));
        }

        [Test]
        public void ChangeLoginAsync_ThrowArgumentNullException_When_PassingNullLogin()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.ChangeLoginAsync(1, null));
        }

        [Test]
        public void ChangePasswordAsync_ThrowArgumentNullException_When_PassingNullOldOrNewPassword()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.ChangePasswordAsync(1, this.validPassword, null));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.ChangePasswordAsync(1, null, this.validPassword));
            Assert.ThrowsAsync<ArgumentNullException>(() => this.repository.ChangePasswordAsync(1, null, null));
        }

        [Test]
        public async Task ChangeLoginAsync_ThrowDbAccountRepositoryException_When_PassingNotExistingUserId_And_RepoIsNotEmpty()
        {
            Account createdAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            Assert.ThrowsAsync<DbAccountRepositoryException>(
                () => this.repository.ChangeLoginAsync(createdAccount.UserId + 1, this.anotherValidLogin));
            Account sameAccount = await this.repository.GetAccountAsync(this.validLogin, this.validPassword);
            Assert.NotNull(sameAccount);
            Assert.AreEqual(createdAccount.UserId, sameAccount.UserId);
        }

        [Test]
        public async Task ChangePasswordAsync_ReturnFalse_When_PassingNotExistingUserId_And_RepoIsNotEmpty()
        {
            Account createdAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            bool changed = await this.repository.ChangePasswordAsync(createdAccount.UserId + 1, this.validPassword, this.anotherValidPassword);
            Assert.IsFalse(changed);
            Account sameAccount = await this.repository.GetAccountAsync(this.validLogin, this.validPassword);
            Assert.NotNull(sameAccount);
            Assert.AreEqual(createdAccount.UserId, sameAccount.UserId);
        }

        [Test]
        public async Task ChangePasswordAsync_ReturnFalse_When_PassingNotExistingUserId_And_RepoIsEmpty()
        {
            bool changed = await this.repository.ChangePasswordAsync(1, this.validPassword, this.anotherValidPassword);
            Assert.IsFalse(changed);
        }

        [Test]
        public async Task ChangeLoginAsync_ReturnTrue_When_PassingValidUserIdAndLogin()
        {
            Account createdAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            bool changed = await this.repository.ChangeLoginAsync(createdAccount.UserId, this.anotherValidLogin);
            Assert.IsTrue(changed);
        }

        [Test]
        public async Task ChangePasswordAsync_ReturnTrue_When_PassingValidUserIdAndLogin()
        {
            Account createdAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            bool changed = await this.repository.ChangePasswordAsync(createdAccount.UserId, this.validPassword, this.anotherValidPassword);
            Assert.IsTrue(changed);
        }

        [Test]
        public async Task ChangeLoginAsync_PerformChangingForGetAccountAsyncResult_When_PassingValidUserIdAndLogin()
        {
            Account createdAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            bool changed = await this.repository.ChangeLoginAsync(createdAccount.UserId, this.anotherValidLogin);
            Account changedAccount = await this.repository.GetAccountAsync(createdAccount.UserId);
            Assert.AreEqual(changedAccount.Login, this.anotherValidLogin);
            Account sameChangedAccount = await this.repository.GetAccountAsync(this.anotherValidLogin, this.validPassword);
            Assert.AreEqual(sameChangedAccount.UserId, createdAccount.UserId);
            Account notValidAccount = await this.repository.GetAccountAsync(this.validLogin, this.validPassword);
            Assert.IsNull(notValidAccount);
        }

        [Test]
        public async Task ChangePasswordAsync_PerformChangingForGetAccountAsyncResult_When_PassingValidUserIdAndLogin()
        {
            Account createdAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            bool changed = await this.repository.ChangePasswordAsync(createdAccount.UserId, this.validPassword, this.anotherValidPassword);
            Account changedAccount = await this.repository.GetAccountAsync(this.validLogin, this.anotherValidPassword);
            Assert.AreEqual(changedAccount.UserId, createdAccount.UserId);
            Account notValidAccount = await this.repository.GetAccountAsync(this.validLogin, this.validPassword);
            Assert.IsNull(notValidAccount);
        }

        [Test]
        public async Task ChangePasswordAsync_ReturnFalse_When_PassingNotValidOldPassword()
        {
            Account createdAccount = await this.repository.CreateAccountAsync(this.validLogin, this.validPassword);
            bool changed = await this.repository.ChangePasswordAsync(
                createdAccount.UserId, new Password(this.validPassword.Value + "a"), this.anotherValidPassword);
            Assert.IsFalse(changed);
            Account sameAccount = await this.repository.GetAccountAsync(this.validLogin, this.validPassword);
            Assert.NotNull(sameAccount);
            Assert.AreEqual(createdAccount.UserId, sameAccount.UserId);
        }
    }
}
