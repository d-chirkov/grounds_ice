using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using GroundsIce.Model.Repositories;

namespace GroundsIce.Model.Accounting.Tests
{
    [TestFixture]
    public class AccountRegistratorTests
    {
        [Test]
        public void WhenCreateRegistratorWithNoValidators()
        {
            var repoMock = new Mock<IAccountRepository>();
            Assert.DoesNotThrow(() => new Registrator(repoMock.Object, null));
        }

        [Test]
        public void WhenCannotCreateRegistratorWithNoRepo()
        {
            Assert.Throws<ArgumentNullException>(() => new Registrator(null, null));
        }

        struct UserData: IEquatable<Account>
        {
            public UserData(User user, Credentials creds)
            {
                User = user;
                Credentials = creds;
            }
            public User User;
            public Credentials Credentials;

            public bool Equals(Account other)
            {
                return other != null && other.User.Equals(User) && other.Credentials.Equals(Credentials);
            }
        }

        [Test]
        public async Task WhenRegisteringNewUserAsync()
        {
            var testAccount = new Account(new User(0), new Credentials("user", "password"));
            var repoMock = new Mock<IAccountRepository>();
            repoMock
                .Setup(c => c.AddNewAccountAsync(It.IsAny<Credentials>()))
                .ReturnsAsync((Credentials creds) => testAccount);
            Registrator registrator = new Registrator(repoMock.Object, null);
            Account newAccount = await registrator.RegisterAsync(testAccount.Credentials);
            Assert.AreEqual(newAccount, testAccount);
            repoMock.Verify(c => c.AddNewAccountAsync(testAccount.Credentials));
        }

        [Test]
        public async Task WhenPassingRegistrationInfoValidation()
        {
            var testAccount = new Account(new User(0), new Credentials("user", "password"));
            var validatorMock = new Mock<ICredentialsValidator>();
            validatorMock
                .Setup(c => c.Validate(It.IsAny<Credentials>()))
                .Verifiable();
            var repoMock = new Mock<IAccountRepository>();
            repoMock
                .Setup(c => c.AddNewAccountAsync(It.IsAny<Credentials>()))
                .ReturnsAsync((Credentials creds) => testAccount);
            Registrator registrator = new Registrator(repoMock.Object, new[] { validatorMock.Object });
            Account newAccount = await registrator.RegisterAsync(testAccount.Credentials);
            Assert.AreEqual(newAccount, testAccount);
            validatorMock.Verify(c => c.Validate(testAccount.Credentials));
        }

        [Test]
        public void WhenFailingValidationOfRegistrationInfo()
        {
            var testAccount = new Account(new User(0), new Credentials("user", "password"));
            string exceptionMessage = "WhenFailingValidationOfRegistrationInfo: test registration exception";
            var validatorMock = new Mock<ICredentialsValidator>();
            validatorMock
                .Setup(c => c.Validate(It.IsAny<Credentials>()))
                .Throws(new CredentialsValidatorException(exceptionMessage));
            var repoMock = new Mock<IAccountRepository>();
            repoMock
                .Setup(c => c.AddNewAccountAsync(It.IsAny<Credentials>()))
                .ThrowsAsync(new Exception("Unreachable code reached"));
            Registrator registrator = new Registrator(repoMock.Object, new[] { validatorMock.Object });
            var exception = Assert.ThrowsAsync<CredentialsValidatorException>(
                async () => await registrator.RegisterAsync(testAccount.Credentials));
            Assert.AreEqual(exception.Message, exceptionMessage);
        }
    }
}
