namespace GroundsIce.Model.Entities.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class AccountTests
    {
        private long userId;

        [SetUp]
        public void SetUp()
        {
            this.userId = 0;
        }

        [Test]
        public void Ctor_ThrowArgumentOutOfRangeException_When_UserIdIsLessThenZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Account(-1, new Login(string.Empty)));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Account(-1, null));
        }

        [Test]
        public void Ctor_ThrowArgumentNullException_When_LoginIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Account(this.userId, null));
        }

        [Test]
        public void Ctor_Create_When_LoginIsNotNullAndUserIdIsGreaterThenZero()
        {
            Assert.DoesNotThrow(() => new Account(this.userId, new Login("a")));
        }

        [Test]
        public void Ctor_SaveUserAndLoginAsProperties_When_LoginIsNotNullAndUserIdIsGreaterThenZero()
        {
            var login = new Login("a");
            var account = new Account(this.userId, login);
            Assert.AreEqual(this.userId, account.UserId);
            Assert.AreEqual(login, account.Login);
        }
    }
}
