using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using GroundsIce.Model.Repositories;

namespace GroundsIce.Model.Accounting.Tests
{
    [TestFixture]
    public class AccountServiceTests
    {
        private User expectedUser_;
        private string username_;
        private string password_;

        [SetUp]
        public void SetUp()
        {
            expectedUser_ = new User(0);
            username_ = "user";
            password_ = "password";
        }

        [Test]
        public void WhenCreateWithNoValidators()
        {
            var repoMock = new Mock<IAccountRepository>();
            Assert.DoesNotThrow(() => new AccountService(repoMock.Object));
        }

        [Test]
        public void WhenCannotCreateRegistratorWithNoRepo()
        {
            Assert.Throws<ArgumentNullException>(() => new AccountService(null));
        }

        [Test]
        public async Task WhenRegisteringNewUserAsync()
        {
            var repoMock = new Mock<IAccountRepository>();
            repoMock
                .Setup(c => c.AddNewUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string username, string password) => expectedUser_);
            AccountService service = new AccountService(repoMock.Object);
            User createdUser = await service.RegisterUserAsync(username_, password_);
            Assert.AreEqual(expectedUser_, createdUser);
            repoMock.Verify(c => c.AddNewUserAsync(username_, password_));
        }

        private async Task WhenPassingValidationTemplate(
            Action<AccountService, ICredentialsValidator> setupService, 
            Action<Mock<ICredentialsValidator>> checkValidator)
        {
            var validator = new Mock<ICredentialsValidator>();
            validator
                .Setup(c => c.Validate(It.IsAny<string>()))
                .Returns(true);
            var repoMock = new Mock<IAccountRepository>();
            repoMock
                .Setup(c => c.AddNewUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string username, string password) => expectedUser_);
            AccountService service = new AccountService(repoMock.Object);
            setupService(service, validator.Object);
            User createdUser = await service.RegisterUserAsync(username_, password_);
            Assert.AreEqual(expectedUser_, createdUser);
            checkValidator(validator);
        }

        [Test]
        public async Task WhenPassingUsernameValidation()
        {
            await WhenPassingValidationTemplate(
                (service, validator) => service.AddUsernameValidator(validator),
                validatorMock => validatorMock.Verify(c => c.Validate(username_)));
        }

        [Test]
        public async Task WhenPassingPasswordValidation()
        {
            await WhenPassingValidationTemplate(
                (service, validator) => service.AddPasswordValidator(validator),
                validatorMock => validatorMock.Verify(c => c.Validate(password_)));
        }

        private void WhenFailingValidationTemplate(Action<AccountService, ICredentialsValidator> setup)
        {
            var validatorMock = new Mock<ICredentialsValidator>();
            validatorMock
                .Setup(c => c.Validate(It.IsAny<string>()))
                .Returns(false);
            var repoMock = new Mock<IAccountRepository>();
            repoMock
                .Setup(c => c.AddNewUserAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Unreachable code reached"));
            var service = new AccountService(repoMock.Object);
            setup(service, validatorMock.Object);
            Assert.ThrowsAsync<CredentialsValidationException>(async () => await service.RegisterUserAsync(username_, password_));
        }

        [Test]
        public void WhenFailingUsernameValidation()
        {
            WhenFailingValidationTemplate((service, validator) => service.AddUsernameValidator(validator));
        }

        [Test]
        public void WhenFailingPasswordValidation()
        {
            WhenFailingValidationTemplate((service, validator) => service.AddPasswordValidator(validator));
        }

        [Test]
        public async Task WhenFailingAddingUserWhileConflict()
        {
            var repoMock = new Mock<IAccountRepository>();
            User expectedUser = null;
            repoMock
                .Setup(c => c.AddNewUserAsync(username_, password_))
                .ReturnsAsync(expectedUser);
            AccountService service = new AccountService(repoMock.Object);
            var createdUser = await service.RegisterUserAsync(username_, password_);
            Assert.True(createdUser == null);
        }

        [Test]
        public async Task WhenGettingUsername()
        {
            var repoMock = new Mock<IAccountRepository>();
            repoMock
                .Setup(c => c.GetUsernameAsync(expectedUser_))
                .ReturnsAsync(username_);
            AccountService service = new AccountService(repoMock.Object);
            Assert.AreEqual(await service.GetUsernameAsync(expectedUser_), username_);
        }
    }
}
