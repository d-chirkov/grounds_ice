using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using GroundsIce.Model.Abstractions.Repositories;
using GroundsIce.WebApi.DTO.Common;
using GroundsIce.Model.Abstractions.Validators;
using System.Net.Http;


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
				.Throws(new UserIdNotFoundException());
            _accountRepositoryMock
                .Setup(e => e.ChangeLoginAsync(_validUserId, _validNewLogin))
                .ReturnsAsync(true);
			_accountRepositoryMock
				.Setup(e => e.ChangeLoginAsync(_validUserId, It.Is<string>(v => v != _validNewLogin)))
				.ReturnsAsync(false);
            _accountRepositoryMock
                .Setup(e => e.ChangePasswordAsync(It.Is<long>(v => v != _validUserId), It.IsAny<string>()))
                .Throws(new UserIdNotFoundException());
			_accountRepositoryMock
                .Setup(e => e.ChangePasswordAsync(_validUserId, It.IsAny<string>()))
                .Returns(Task.FromResult(0));

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

		//Empty
		private DTO.Credentials DTOE()
		{
			return new DTO.Credentials(null, null);
		}

		//Login
		private DTO.Credentials DTOL(string login)
		{
			return new DTO.Credentials(login, null);
		}

		//Password
		private DTO.Credentials DTOP(string password)
		{
			return new DTO.Credentials(null, password);
		}

		//Both
		private DTO.Credentials DTOB(string login, string password)
		{
			return new DTO.Credentials(login, password);
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
			Assert.ThrowsAsync<ArgumentNullException>(() => _accountController.Register(null));
			Assert.ThrowsAsync<ArgumentNullException>(() => _accountController.Register(DTOE()));
			Assert.ThrowsAsync<ArgumentNullException>(() => _accountController.Register(DTOL(_validLogin)));
			Assert.ThrowsAsync<ArgumentNullException>(() => _accountController.Register(DTOP(_validPassword)));
		}

		[Test]
		public async Task Register_ReturnsSuccess_When_PassingValidLoginAndPassword()
		{
			Value value = await _accountController.Register(DTOB(_validLogin, _validPassword));
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
			_accountRepositoryMock.Verify(v => v.CreateAccountAsync(_validLogin, _validPassword));
		}

		[Test]
		public async Task Register_ReturnsSuccess_When_PassingValidLoginAndPasswordThroughLoginValidator()
		{
			_accountController.SetLoginValidators(new []{ _validatorMock.Object });
			Value value = await _accountController.Register(DTOB(_validLogin, _validPassword));
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
				() => _accountController.Register(DTOB(_invalidString, _validPassword)));
		}

		[Test]
		public async Task ChangeLogin_ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator()
		{
			SetUserIdToRequest(_validUserId);
			await _ReturnsLoginNotValid_When_PassingInvalidLoginThroughValidator(
				() => _accountController.ChangeLogin(DTOL(_invalidString)));
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
				() => _accountController.Register(DTOB(_validLogin, _invalidString)));
		}

		[Test]
		public async Task ChangePassword_ReturnsPasswordNotValid_When_PassingInvalidLoginThroughValidator()
		{
			SetUserIdToRequest(_validUserId);
			await _ReturnsPasswordNotValid_When_PassingInvalidPasswordThroughValidator(
				() => _accountController.ChangePassword(DTOP(_invalidString)));
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
				() => _accountController.Register(DTOB(_alreadyExistingLogin, _validPassword)));
			_accountRepositoryMock.Verify(v => v.CreateAccountAsync(_alreadyExistingLogin, _validPassword));
		}

		[Test]
		public async Task ChangeLogin_ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin()
		{
			SetUserIdToRequest(_validUserId);
			await _ReturnsLoginAlreadyExists_When_PassingAlreadyExistingLogin(
				() => _accountController.ChangeLogin(DTOL(_alreadyExistingLogin)));
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

		private void _ReturnsAccountNotExistsWithNullPayload_When_RequestedAccountNotExistsInRepo(Func<Value> func)
		{
			SetUserIdToRequest(_notExistingUserId);
			Value value = func();
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.AccountNotExists);
			Assert.IsNull(value.Payload);
		}

		[Test]
		public void GetAccount_ReturnsAccountNotExistsWithNullPayload_When_RequestedAccountNotExistsInRepo()
		{
			_ReturnsAccountNotExistsWithNullPayload_When_RequestedAccountNotExistsInRepo(() => _accountController.GetAccount().Result);
		}

		[Test]
		public void ChangeLogin_ReturnsAccountNotExistsWithNullAccount_When_RequestedAccountNotExistsInRepo()
		{
			_ReturnsAccountNotExistsWithNullPayload_When_RequestedAccountNotExistsInRepo(
				() => _accountController.ChangeLogin(DTOL(_validNewLogin)).Result);
		}

		[Test]
		public void ChangePassword_ReturnsAccountNotExistsWithNullAccount_When_RequestedAccountNotExistsInRepo()
		{
			_ReturnsAccountNotExistsWithNullPayload_When_RequestedAccountNotExistsInRepo(
				() => _accountController.ChangePassword(DTOP("a")).Result);
		}

		[Test]
		public void GetAccount_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
		{
			Assert.ThrowsAsync<ArgumentNullException>(async () => await _accountController.GetAccount());
		}

		private void _ThrowArgumentNullException_When_ArgumentIsNull(Func<DTO.Credentials, Task<Value>> func)
		{
			SetUserIdToRequest(_validUserId);
			Assert.ThrowsAsync<ArgumentNullException>(async () => await func(null));
			Assert.ThrowsAsync<ArgumentNullException>(async () => await func(DTOE()));
		}

		[Test]
		public void ChangeLogin_ThrowArgumentNullException_When_ArgumentIsNull()
		{
			_ThrowArgumentNullException_When_ArgumentIsNull(_accountController.ChangeLogin);
		}

		[Test]
		public void ChangePassword_ThrowArgumentNullException_When_ArgumentIsNull()
		{
			_ThrowArgumentNullException_When_ArgumentIsNull(_accountController.ChangePassword);
		}

		private void _ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest(Func<DTO.Credentials, Task<Value>> func)
		{
			Assert.ThrowsAsync<ArgumentNullException>(async () => await func(DTOB(_validLogin, _validPassword)));
		}

		[Test]
		public void ChangeLogin_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
		{
			_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest(_accountController.ChangeLogin);
		}

		[Test]
		public void ChangePassword_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
		{
			_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest(_accountController.ChangePassword);
		}

		[Test]
		public async Task ChangeLogin_ReturnsSuccess_When_PassingValidNewLogin()
		{
			SetUserIdToRequest(_validUserId);
			Value value = await _accountController.ChangeLogin(DTOL(_validNewLogin));
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
			_accountRepositoryMock.Verify(v => v.ChangeLoginAsync(_validUserId, _validNewLogin));
		}

		[Test]
		public async Task ChangePassword_ReturnsSuccess_When_PassingValidNewPassword()
		{
			SetUserIdToRequest(_validUserId);
			string newPassword = "a";
			Value value = await _accountController.ChangePassword(DTOP(newPassword));
			Assert.AreEqual(value.Type, (int)AccountController.ValueType.Success);
			_accountRepositoryMock.Verify(v => v.ChangePasswordAsync(_validUserId, newPassword));
		}

		
	}
}