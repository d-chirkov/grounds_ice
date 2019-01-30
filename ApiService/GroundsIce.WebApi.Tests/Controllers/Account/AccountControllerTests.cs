using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using GroundsIce.Model.Abstractions.Repositories;
using GroundsIce.WebApi.DTO.Common;
using GroundsIce.Model.Abstractions.Validators;
using System.Net.Http;
using System.Linq;


namespace GroundsIce.WebApi.Controllers.Account.Tests
{
	using Account = Model.Entities.Account;

	[TestFixture]
    class AccountControllerTests
    {
        private Mock<IAccountRepository> _accountRepositoryMock;
        private long _validUserId;
		private long _notExistingUserId;
		private string _validLogin;
        private string _validPassword;
        private string _validNewLogin;
		private string _alreadyExistingLogin;
		private AccountController _accountController;
		private Mock<IStringValidator> _validatorMock;
		private string _invalidString;

		[SetUp]
        public void SetUp()
        {
            _validUserId = 1;
			_notExistingUserId = 2;
			_validLogin = "login";
            _validPassword = "password";
            _validNewLogin = "new_login";
			_alreadyExistingLogin = "exists";

			_accountRepositoryMock = new Mock<IAccountRepository>();
            _accountRepositoryMock
                .Setup(e => e.CreateAccountAsync(It.Is<string>(v => v != _validLogin), It.Is<string>(v => v != _validPassword)))
                .ReturnsAsync((Account)null);
            _accountRepositoryMock
                .Setup(e => e.CreateAccountAsync(_validLogin, _validPassword))
                .ReturnsAsync(new Account(_validUserId, _validLogin));
            _accountRepositoryMock
                .Setup(e => e.GetAccountAsync(It.Is<long>(v => v != _validUserId)))
                .ReturnsAsync((Account)null);
            _accountRepositoryMock
                .Setup(e => e.GetAccountAsync(_validUserId))
                .ReturnsAsync(new Account(_validUserId, _validLogin));
            _accountRepositoryMock
                .Setup(e => e.GetAccountAsync(It.Is<string>(v => v != _validLogin), It.Is<string>(v => v != _validPassword)))
                .ReturnsAsync((Account)null);
            _accountRepositoryMock
                .Setup(e => e.GetAccountAsync(_validLogin, _validPassword))
                .ReturnsAsync(new Account(_validUserId, _validLogin));
            _accountRepositoryMock
                .Setup(e => e.ChangeLoginAsync(It.Is<long>(v => v != _validUserId), It.IsAny<string>()))
				.ThrowsAsync(new Exception());
            _accountRepositoryMock
                .Setup(e => e.ChangeLoginAsync(_validUserId, _validNewLogin))
                .ReturnsAsync(true);
			_accountRepositoryMock
				.Setup(e => e.ChangeLoginAsync(_validUserId, It.Is<string>(v => v != _validNewLogin)))
				.ReturnsAsync(false);
            _accountRepositoryMock
                .Setup(e => e.ChangePasswordAsync(It.Is<long>(v => v != _validUserId), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);
			_accountRepositoryMock
				.Setup(e => e.ChangePasswordAsync(_validUserId, It.Is<string>(v => v != _validPassword), It.IsAny<string>()))
				.ReturnsAsync(false);
			_accountRepositoryMock
                .Setup(e => e.ChangePasswordAsync(_validUserId, _validPassword, It.IsAny<string>()))
                .ReturnsAsync(true);

			_accountController = new AccountController(_accountRepositoryMock.Object);

			_invalidString = "invalid";
			_validatorMock = new Mock<IStringValidator>();
			_validatorMock.Setup(e => e.ValidateAsync(_invalidString)).ReturnsAsync(false);
			_validatorMock.Setup(e => e.ValidateAsync(It.Is<string>(v => v != _invalidString))).ReturnsAsync(true);
		}

		private void SetUserIdToRequest(long userId)
		{
			_accountController.Request = new HttpRequestMessage();
			_accountController.Request.Properties["USER_ID"] = userId;
		}

		[Test]
        public void Ctor_ThrowArgumentNullException_When_PassingNullRepo()
        {
            Assert.Throws<ArgumentNullException>(() => new AccountController(null));
        }

		private void _ThrowsArgumentNullException_When_PassingNullArrayOfValidators(Action<IEnumerable<IStringValidator>> func)
		{
			Assert.Throws<ArgumentNullException>(() => func(null));
		}

		[Test]
		public void AddLoginValidatorRange_ThrowsArgumentNullException_When_PassingNullArrayOfValidators()
		{
			_ThrowsArgumentNullException_When_PassingNullArrayOfValidators(_accountController.SetLoginValidators);
		}

		[Test]
		public void AddPasswordValidatorRange_ThrowsArgumentNullException_When_PassingNullArrayOfValidators()
		{
			_ThrowsArgumentNullException_When_PassingNullArrayOfValidators(_accountController.SetPasswordValidators);
		}

		private void _ThrowsArgumentNullException_When_PassingNullValidatorInArray(Action<IEnumerable<IStringValidator>> func)
		{
			Assert.Throws<ArgumentNullException>(() => func(new IStringValidator[] { null }));
		}

		[Test]
		public void AddLoginValidatorRange_ThrowsArgumentNullException_When_PassingNullValidatorInArray()
		{
			_ThrowsArgumentNullException_When_PassingNullValidatorInArray(_accountController.SetLoginValidators);
		}

		[Test]
		public void AddPasswordValidatorRange_ThrowsArgumentNullException_When_PassingNullValidatorInArray()
		{
			_ThrowsArgumentNullException_When_PassingNullValidatorInArray(_accountController.SetPasswordValidators);
		}

		private void _DoesNotThrow_When_PassingNotNullValidatorInArray(Action<IEnumerable<IStringValidator>> func)
		{
			Assert.DoesNotThrow(() => func(new IStringValidator[] { _validatorMock.Object }));
		}

		[Test]
		public void AddLoginValidatorRange_DoesNotThrow_When_PassingNotNullValidatorInArray()
		{
			_DoesNotThrow_When_PassingNotNullValidatorInArray(_accountController.SetLoginValidators);
		}

		[Test]
		public void AddPasswordValidatorRange_DoesNotThrow_When_PassingNotNullValidatorInArray()
		{
			_DoesNotThrow_When_PassingNotNullValidatorInArray(_accountController.SetPasswordValidators);
		}

		[Test]
		public void Register_ThrowsArgumentNullException_When_PassingNullDtoOrLoginOrPassword()
		{
			var args = new List<DTO.LoginAndPassword>
			{
				null,
				new DTO.LoginAndPassword(null, null),
				new DTO.LoginAndPassword(_validLogin, null),
				new DTO.LoginAndPassword(null, _validPassword)
			};
			args.ForEach(arg => Assert.ThrowsAsync<ArgumentNullException>(() => _accountController.Register(arg)));
		}

		[Test]
		public async Task Register_ReturnsSuccess_When_PassingValidLoginAndPassword()
		{
			Value value = await _accountController.Register(new DTO.LoginAndPassword(_validLogin, _validPassword));
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
			_accountRepositoryMock.Verify(v => v.CreateAccountAsync(_validLogin, _validPassword));
		}

		[Test]
		public async Task Register_ReturnsSuccess_When_PassingValidLoginAndPasswordThroughLoginValidator()
		{
			_accountController.SetLoginValidators(new []{ _validatorMock.Object });
			Value value = await _accountController.Register(new DTO.LoginAndPassword(_validLogin, _validPassword));
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
			_validatorMock.Verify(v => v.ValidateAsync(_validLogin));
		}

		private async Task _ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator(Func<Task<Value>> func)
		{
			_accountController.SetLoginValidators(new[] { _validatorMock.Object });
			Value value = await func();
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.LoginNotValid);
			_validatorMock.Verify(v => v.ValidateAsync(_invalidString));
			_accountRepositoryMock.VerifyNoOtherCalls();
		}

		[Test]
		public async Task Register_ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator()
		{
			await _ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator(
				() => _accountController.Register(new DTO.LoginAndPassword(_invalidString, _validPassword)));
		}

		[Test]
		public async Task ChangeLogin_ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator()
		{
			SetUserIdToRequest(_validUserId);
			await _ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator(
				() => _accountController.ChangeLogin(new DTO.NewLogin(_invalidString)));
		}

		private async Task _ReturnsPasswordNotValid_When_PassingInvalidPasswordThroughValidator(Func<Task<Value>> func)
		{
			_accountController.SetPasswordValidators(new[] { _validatorMock.Object });
			Value value = await func();
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.PasswordNotValid);
			_validatorMock.Verify(v => v.ValidateAsync(_invalidString));
			_accountRepositoryMock.VerifyNoOtherCalls();
		}

		[Test]
		public async Task Register_ReturnsPasswordNotValid_When_PassingInvalidPasswordThroughLoginValidator()
		{
			await _ReturnsPasswordNotValid_When_PassingInvalidPasswordThroughValidator(
				() => _accountController.Register(new DTO.LoginAndPassword(_validLogin, _invalidString)));
		}

		[Test]
		public async Task ChangePassword_ReturnsPasswordNotValid_When_PassingInvalidNewPasswordThroughValidator()
		{
			SetUserIdToRequest(_validUserId);
			await _ReturnsPasswordNotValid_When_PassingInvalidPasswordThroughValidator(
				() => _accountController.ChangePassword(new DTO.OldAndNewPasswords("", _invalidString)));
		}

		private async Task _ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin(Func<Task<Value>> func)
		{
			Value value = await func();
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.LoginAlreadyExists);
		}

		[Test]
		public async Task Register_ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin()
		{
			await _ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin(
				() => _accountController.Register(new DTO.LoginAndPassword(_alreadyExistingLogin, _validPassword)));
			_accountRepositoryMock.Verify(v => v.CreateAccountAsync(_alreadyExistingLogin, _validPassword));
		}

		[Test]
		public async Task ChangeLogin_ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin()
		{
			SetUserIdToRequest(_validUserId);
			await _ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin(
				() => _accountController.ChangeLogin(new DTO.NewLogin(_alreadyExistingLogin)));
			_accountRepositoryMock.Verify(v => v.ChangeLoginAsync(_validUserId, _alreadyExistingLogin));
		}

		[Test]
		public async Task GetAccount_ReturnsSuccessWithValidAccount_When_RequestedAccountExistsInRepo()
		{
			SetUserIdToRequest(_validUserId);
			Value<Account> value = await _accountController.GetAccount();
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
			Assert.AreEqual(value.Payload.Login, _validLogin);
			Assert.AreEqual(value.Payload.UserId, _validUserId);
		}

		private void _ReturnsAccountControllerValueType_When_RequestedAccountNotExistsInRepo(
			Func<Value> func,
			AccountController.ValueType expectedValueType)
		{
			SetUserIdToRequest(_notExistingUserId);
			Value value = func();
			Assert.AreEqual(value.Type, (int)expectedValueType);
			Assert.IsNull(value.Payload);
		}

		[Test]
		public void GetAccount_ReturnsAccountNotExistsWithNullPayload_When_RequestedAccountNotExistsInRepo()
		{
			_ReturnsAccountControllerValueType_When_RequestedAccountNotExistsInRepo(
				() => _accountController.GetAccount().Result, 
				AccountController.ValueType.AccountNotExists);
		}

		[Test]
		public void ChangeLogin_ThrowsException_When_RequestedAccountNotExistsInRepo()
		{
			SetUserIdToRequest(_notExistingUserId);
			Assert.ThrowsAsync<Exception>(() => _accountController.ChangeLogin(new DTO.NewLogin(_validNewLogin)));
		}

		[Test]
		public void ChangePassword_ReturnsAccountOldPasswordNotValid_When_RequestedAccountNotExistsInRepo()
		{
			_ReturnsAccountControllerValueType_When_RequestedAccountNotExistsInRepo(
				() => _accountController.ChangePassword(new DTO.OldAndNewPasswords("a", "b")).Result,
				AccountController.ValueType.OldPasswordNotValid);
		}

		[Test]
		public void GetAccount_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _accountController.GetAccount());
		}
		
		[Test]
		public void ChangeLogin_ThrowArgumentNullException_When_ArgumentIsNull()
		{
			SetUserIdToRequest(_validUserId);
			var args = new List<DTO.NewLogin> {
				null,
				new DTO.NewLogin(null),
			};
			args.ForEach(arg => Assert.ThrowsAsync<ArgumentNullException>(() => _accountController.ChangeLogin(arg)));
		}

		[Test]
		public void ChangePassword_ThrowArgumentNullException_When_ArgumentIsNull()
		{
			SetUserIdToRequest(_validUserId);
			var args = new List<DTO.OldAndNewPasswords> {
				null,
				new DTO.OldAndNewPasswords(null, "a"),
				new DTO.OldAndNewPasswords("a", null),
				new DTO.OldAndNewPasswords(null, null)
			};
			args.ForEach(arg => Assert.ThrowsAsync<ArgumentNullException>(() => _accountController.ChangePassword(arg)));
		}

		[Test]
		public void ChangeLogin_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _accountController.ChangeLogin(new DTO.NewLogin(_validLogin)));
		}

		[Test]
		public void ChangePassword_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
		{
			Assert.ThrowsAsync<ArgumentNullException>(
				() => _accountController.ChangePassword(new DTO.OldAndNewPasswords(_validPassword, _validPassword + "a")));
		}

		[Test]
		public async Task ChangeLogin_ReturnsSuccess_When_PassingValidNewLogin()
		{
			SetUserIdToRequest(_validUserId);
			Value value = await _accountController.ChangeLogin(new DTO.NewLogin(_validNewLogin));
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
			_accountRepositoryMock.Verify(v => v.ChangeLoginAsync(_validUserId, _validNewLogin));
		}

		[Test]
		public async Task ChangePassword_ReturnsSuccess_When_PassingValidOldAndNewPassword()
		{
			SetUserIdToRequest(_validUserId);
			string newPassword = "a";
			Value value = await _accountController.ChangePassword(new DTO.OldAndNewPasswords(_validPassword, newPassword));
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
			_accountRepositoryMock.Verify(v => v.ChangePasswordAsync(_validUserId, _validPassword, newPassword));
		}

		[Test]
		public async Task ChangePassword_ReturnsOldPasswordNotValid_When_PassingInvalidOldPassword()
		{
			SetUserIdToRequest(_validUserId);
			string newPassword = "a";
			Value value = await _accountController.ChangePassword(new DTO.OldAndNewPasswords(_validPassword + "a", newPassword));
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.OldPasswordNotValid);
			_accountRepositoryMock.Verify(v => v.ChangePasswordAsync(_validUserId, _validPassword + "a", newPassword));
		}
	}
}