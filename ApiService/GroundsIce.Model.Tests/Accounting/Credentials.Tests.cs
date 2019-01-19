using NUnit.Framework;
using System;

namespace GroundsIce.Model.Accounting.Tests
{
    [TestFixture]
    class CredentialsTests
    {
        [Test]
        public void WhenCreatingEmptyCredentials()
        {
            Assert.DoesNotThrow(() => new Credentials("", ""));
        }

        [Test]
        public void WhenCreatingCredentials()
        {
            Assert.DoesNotThrow(() => new Credentials("user", "password"));
        }

        [Test]
        public void WhenCannotCreateNullCredentials()
        {
            Assert.Throws<ArgumentNullException>(() => new Credentials(null, "password"));
            Assert.Throws<ArgumentNullException>(() => new Credentials("user", null));
            Assert.Throws<ArgumentNullException>(() => new Credentials(null, null));
        }

        [Test]
        public void WhenAccessingCredentials()
        {
            var credentials = new Credentials("user", "password");
            Assert.AreEqual(credentials.Name, "user");
            Assert.AreEqual(credentials.Password, "password");
        }

        [Test]
        public void WhenSameCredentialsAreEquals()
        {
            var credentials1 = new Credentials("user", "password");
            var credentials2 = new Credentials("user", "password");
            Assert.AreEqual(credentials1, credentials2);
        }

        [Test]
        public void WhenDifferentCredentialsAreNotEqualsByPassword()
        {
            var credentials1 = new Credentials("user1", "password1");
            var credentials2 = new Credentials("user1", "password2");
            Assert.AreNotEqual(credentials1, credentials2);
        }

        [Test]
        public void WhenDifferentCredentialsAreNotEqualsByName()
        {
            var credentials1 = new Credentials("user1", "password1");
            var credentials2 = new Credentials("user2", "password1");
            Assert.AreNotEqual(credentials1, credentials2);
        }

        [Test]
        public void WhenDifferentCredentialsAreNotEqualsByNameAndPassword()
        {
            var credentials1 = new Credentials("user1", "password1");
            var credentials2 = new Credentials("user2", "password2");
            Assert.AreNotEqual(credentials1, credentials2);
        }
    }
}
