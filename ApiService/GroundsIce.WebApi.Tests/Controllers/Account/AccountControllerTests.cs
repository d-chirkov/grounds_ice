namespace GroundsIce.WebApi.Controllers.Account.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using GroundsIce.Model.Abstractions.Repositories;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.WebApi.DTO.Common;
    using Moq;
    using NUnit.Framework;
    using Account_OLD = Model.Entities.Account_OLD;

    [TestFixture]
    public class AccountControllerTests
    {
        private Mock<IAccountRepository_OLD> accountRepositoryMock;
        private long validUserId;
        private long notExistingUserId;
        private string validLogin;
        private string validPassword;
        private string validNewLogin;
        private string alreadyExistingLogin;
        private AccountController accountController;
        private Mock<ILoginValidator> loginValidatorMock;
        private Mock<IPasswordValidator> passwordValidatorMock;
        private string invalidString;

        [SetUp]
        public void SetUp()
        {
            this.validUserId = 1;
            this.notExistingUserId = 2;
            this.validLogin = "login";
            this.validPassword = "password";
            this.validNewLogin = "new_login";
            this.alreadyExistingLogin = "exists";

            this.accountRepositoryMock = new Mock<IAccountRepository_OLD>();
            this.accountRepositoryMock
                .Setup(e => e.CreateAccountAsync(It.Is<string>(v => v != this.validLogin), It.Is<string>(v => v != this.validPassword)))
                .ReturnsAsync((Account_OLD)null);
            this.accountRepositoryMock
                .Setup(e => e.CreateAccountAsync(this.validLogin, this.validPassword))
                .ReturnsAsync(new Account_OLD(this.validUserId, this.validLogin));
            this.accountRepositoryMock
                .Setup(e => e.GetAccountAsync(It.Is<long>(v => v != this.validUserId)))
                .ReturnsAsync((Account_OLD)null);
            this.accountRepositoryMock
                .Setup(e => e.GetAccountAsync(this.validUserId))
                .ReturnsAsync(new Account_OLD(this.validUserId, this.validLogin));
            this.accountRepositoryMock
                .Setup(e => e.GetAccountAsync(It.Is<string>(v => v != this.validLogin), It.Is<string>(v => v != this.validPassword)))
                .ReturnsAsync((Account_OLD)null);
            this.accountRepositoryMock
                .Setup(e => e.GetAccountAsync(this.validLogin, this.validPassword))
                .ReturnsAsync(new Account_OLD(this.validUserId, this.validLogin));
            this.accountRepositoryMock
                .Setup(e => e.ChangeLoginAsync(It.Is<long>(v => v != this.validUserId), It.IsAny<string>()))
                .ThrowsAsync(new Exception());
            this.accountRepositoryMock
                .Setup(e => e.ChangeLoginAsync(this.validUserId, this.validNewLogin))
                .ReturnsAsync(true);
            this.accountRepositoryMock
                .Setup(e => e.ChangeLoginAsync(this.validUserId, It.Is<string>(v => v != this.validNewLogin)))
                .ReturnsAsync(false);
            this.accountRepositoryMock
                .Setup(e => e.ChangePasswordAsync(It.Is<long>(v => v != this.validUserId), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);
            this.accountRepositoryMock
                .Setup(e => e.ChangePasswordAsync(this.validUserId, It.Is<string>(v => v != this.validPassword), It.IsAny<string>()))
                .ReturnsAsync(false);
            this.accountRepositoryMock
                .Setup(e => e.ChangePasswordAsync(this.validUserId, this.validPassword, It.IsAny<string>()))
                .ReturnsAsync(true);

            this.invalidString = "invalid";
            this.loginValidatorMock = new Mock<ILoginValidator>();
            this.loginValidatorMock.Setup(e => e.ValidateAsync(this.invalidString)).ReturnsAsync(false);
            this.loginValidatorMock.Setup(e => e.ValidateAsync(It.Is<string>(v => v != this.invalidString))).ReturnsAsync(true);
            this.passwordValidatorMock = new Mock<IPasswordValidator>();
            this.passwordValidatorMock.Setup(e => e.ValidateAsync(this.invalidString)).ReturnsAsync(false);
            this.passwordValidatorMock.Setup(e => e.ValidateAsync(It.Is<string>(v => v != this.invalidString))).ReturnsAsync(true);

            this.accountController = new AccountController(this.accountRepositoryMock.Object);
        }

        public void SetUserIdToRequest(long userId, AccountController accountController = null)
        {
            if (accountController == null)
            {
                accountController = this.accountController;
            }

            accountController.Request = new HttpRequestMessage();
            accountController.Request.Properties["USER_ID"] = userId;
        }

        [Test]
        public void Ctor_ThrowArgumentNullException_When_PassingNullRepo()
        {
            Assert.Throws<ArgumentNullException>(() => new AccountController(null));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingNotNullLoginValidator()
        {
            Assert.DoesNotThrow(() => new AccountController(this.accountRepositoryMock.Object, this.loginValidatorMock.Object));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingNotNullPasswordValidator()
        {
            Assert.DoesNotThrow(() => new AccountController(this.accountRepositoryMock.Object, null, this.passwordValidatorMock.Object));
        }

        [Test]
        public void Register_ThrowsArgumentNullException_When_PassingNullDtoOrLoginOrPassword()
        {
            var args = new List<DTO.LoginAndPassword>
            {
                null,
                new DTO.LoginAndPassword(null, null),
                new DTO.LoginAndPassword(this.validLogin, null),
                new DTO.LoginAndPassword(null, this.validPassword)
            };
            args.ForEach(arg => Assert.ThrowsAsync<ArgumentNullException>(() => this.accountController.Register(arg)));
        }

        [Test]
        public async Task Register_ReturnsSuccess_When_PassingValidLoginAndPassword()
        {
            Value value = await this.accountController.Register(new DTO.LoginAndPassword(this.validLogin, this.validPassword));
            Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
            this.accountRepositoryMock.Verify(v => v.CreateAccountAsync(this.validLogin, this.validPassword));
        }

        [Test]
        public async Task Register_ReturnsSuccess_When_PassingValidLoginAndPasswordThroughLoginValidator()
        {
            var subject = new AccountController(
                this.accountRepositoryMock.Object,
                this.loginValidatorMock.Object);
            Value value = await subject.Register(new DTO.LoginAndPassword(this.validLogin, this.validPassword));
            Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
            this.loginValidatorMock.Verify(v => v.ValidateAsync(this.validLogin));
        }

        public async Task ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator(Func<Task<Value>> func)
        {
            var subject = new AccountController(
                this.accountRepositoryMock.Object,
                this.loginValidatorMock.Object);
            Value value = await func();
            Assert.AreEqual(value.Type, (int)AccountController.ValueType.LoginNotValid);
            this.loginValidatorMock.Verify(v => v.ValidateAsync(this.invalidString));
            this.accountRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Register_ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator()
        {
            var subject = new AccountController(
                this.accountRepositoryMock.Object,
                this.loginValidatorMock.Object);
            await this.ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator(
                () => subject.Register(new DTO.LoginAndPassword(this.invalidString, this.validPassword)));
        }

        [Test]
        public async Task ChangeLogin_ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator()
        {
            var subject = new AccountController(
                this.accountRepositoryMock.Object,
                this.loginValidatorMock.Object);
            this.SetUserIdToRequest(this.validUserId, subject);
            await this.ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator(
                () => subject.ChangeLogin(new DTO.NewLogin(this.invalidString)));
        }

        public async Task ReturnsPasswordNotValid_When_PassingInvalidPasswordThroughValidator(Func<Task<Value>> func)
        {
            Value value = await func();
            Assert.AreEqual(value.Type, (int)AccountController.ValueType.PasswordNotValid);
            this.passwordValidatorMock.Verify(v => v.ValidateAsync(this.invalidString));
            this.accountRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task Register_ReturnsPasswordNotValid_When_PassingInvalidPasswordThroughLoginValidator()
        {
            var subject = new AccountController(
                this.accountRepositoryMock.Object,
                null,
                this.passwordValidatorMock.Object);
            await this.ReturnsPasswordNotValid_When_PassingInvalidPasswordThroughValidator(
                () => subject.Register(new DTO.LoginAndPassword(this.validLogin, this.invalidString)));
        }

        [Test]
        public async Task ChangePassword_ReturnsPasswordNotValid_When_PassingInvalidNewPasswordThroughValidator()
        {
            var subject = new AccountController(
                this.accountRepositoryMock.Object,
                null,
                this.passwordValidatorMock.Object);
            this.SetUserIdToRequest(this.validUserId, subject);
            await this.ReturnsPasswordNotValid_When_PassingInvalidPasswordThroughValidator(
                () => subject.ChangePassword(new DTO.OldAndNewPasswords(string.Empty, this.invalidString)));
        }

        public async Task ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin(Func<Task<Value>> func)
        {
            Value value = await func();
            Assert.AreEqual(value.Type, (int)AccountController.ValueType.LoginAlreadyExists);
        }

        [Test]
        public async Task Register_ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin()
        {
            await this.ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin(
                () => this.accountController.Register(new DTO.LoginAndPassword(this.alreadyExistingLogin, this.validPassword)));
            this.accountRepositoryMock.Verify(v => v.CreateAccountAsync(this.alreadyExistingLogin, this.validPassword));
        }

        [Test]
        public async Task ChangeLogin_ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin()
        {
            this.SetUserIdToRequest(this.validUserId);
            await this.ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin(
                () => this.accountController.ChangeLogin(new DTO.NewLogin(this.alreadyExistingLogin)));
            this.accountRepositoryMock.Verify(v => v.ChangeLoginAsync(this.validUserId, this.alreadyExistingLogin));
        }

        [Test]
        public async Task GetAccount_ReturnsSuccessWithValidAccount_When_RequestedAccountExistsInRepo()
        {
            this.SetUserIdToRequest(this.validUserId);
            Value<Account_OLD> value = await this.accountController.GetAccount();
            Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
            Assert.AreEqual(value.Payload.Login, this.validLogin);
            Assert.AreEqual(value.Payload.UserId, this.validUserId);
        }

        public void ReturnsAccountControllerValueType_When_RequestedAccountNotExistsInRepo(
            Func<Value> func,
            AccountController.ValueType expectedValueType)
        {
            this.SetUserIdToRequest(this.notExistingUserId);
            Value value = func();
            Assert.AreEqual(value.Type, (int)expectedValueType);
            Assert.IsNull(value.Payload);
        }

        [Test]
        public void GetAccount_ReturnsAccountNotExistsWithNullPayload_When_RequestedAccountNotExistsInRepo()
        {
            this.ReturnsAccountControllerValueType_When_RequestedAccountNotExistsInRepo(
                () => this.accountController.GetAccount().Result,
                AccountController.ValueType.AccountNotExists);
        }

        [Test]
        public void ChangeLogin_ThrowsException_When_RequestedAccountNotExistsInRepo()
        {
            this.SetUserIdToRequest(this.notExistingUserId);
            Assert.ThrowsAsync<Exception>(() => this.accountController.ChangeLogin(new DTO.NewLogin(this.validNewLogin)));
        }

        [Test]
        public void ChangePassword_ReturnsAccountOldPasswordNotValid_When_RequestedAccountNotExistsInRepo()
        {
            this.ReturnsAccountControllerValueType_When_RequestedAccountNotExistsInRepo(
                () => this.accountController.ChangePassword(new DTO.OldAndNewPasswords("a", "b")).Result,
                AccountController.ValueType.OldPasswordNotValid);
        }

        [Test]
        public void GetAccount_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.accountController.GetAccount());
        }

        [Test]
        public void ChangeLogin_ThrowArgumentNullException_When_ArgumentIsNull()
        {
            this.SetUserIdToRequest(this.validUserId);
            var args = new List<DTO.NewLogin>
            {
                null,
                new DTO.NewLogin(null),
            };
            args.ForEach(arg => Assert.ThrowsAsync<ArgumentNullException>(() => this.accountController.ChangeLogin(arg)));
        }

        [Test]
        public void ChangePassword_ThrowArgumentNullException_When_ArgumentIsNull()
        {
            this.SetUserIdToRequest(this.validUserId);
            var args = new List<DTO.OldAndNewPasswords>
            {
                null,
                new DTO.OldAndNewPasswords(null, "a"),
                new DTO.OldAndNewPasswords("a", null),
                new DTO.OldAndNewPasswords(null, null)
            };
            args.ForEach(arg => Assert.ThrowsAsync<ArgumentNullException>(() => this.accountController.ChangePassword(arg)));
        }

        [Test]
        public void ChangeLogin_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.accountController.ChangeLogin(new DTO.NewLogin(this.validLogin)));
        }

        [Test]
        public void ChangePassword_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                () => this.accountController.ChangePassword(new DTO.OldAndNewPasswords(this.validPassword, this.validPassword + "a")));
        }

        [Test]
        public async Task ChangeLogin_ReturnsSuccess_When_PassingValidNewLogin()
        {
            this.SetUserIdToRequest(this.validUserId);
            Value value = await this.accountController.ChangeLogin(new DTO.NewLogin(this.validNewLogin));
            Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
            this.accountRepositoryMock.Verify(v => v.ChangeLoginAsync(this.validUserId, this.validNewLogin));
        }

        [Test]
        public async Task ChangePassword_ReturnsSuccess_When_PassingValidOldAndNewPassword()
        {
            this.SetUserIdToRequest(this.validUserId);
            string newPassword = "a";
            Value value = await this.accountController.ChangePassword(new DTO.OldAndNewPasswords(this.validPassword, newPassword));
            Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
            this.accountRepositoryMock.Verify(v => v.ChangePasswordAsync(this.validUserId, this.validPassword, newPassword));
        }

        [Test]
        public async Task ChangePassword_ReturnsOldPasswordNotValid_When_PassingInvalidOldPassword()
        {
            this.SetUserIdToRequest(this.validUserId);
            string newPassword = "a";
            Value value = await this.accountController.ChangePassword(new DTO.OldAndNewPasswords(this.validPassword + "a", newPassword));
            Assert.AreEqual(value.Type, (int)AccountController.ValueType.OldPasswordNotValid);
            this.accountRepositoryMock.Verify(v => v.ChangePasswordAsync(this.validUserId, this.validPassword + "a", newPassword));
        }
    }
}