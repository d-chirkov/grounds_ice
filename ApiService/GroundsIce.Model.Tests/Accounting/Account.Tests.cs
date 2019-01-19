using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model.Accounting.Tests
{
    [TestFixture]
    class AccountTests
    {
        User user_;
        Credentials credentials_;

        [SetUp]
        public void SetUp()
        {
            user_ = new User(0);
            credentials_ = new Credentials("user", "password");
        }

        [Test]
        public void WhenCreatingAccount()
        {
            Assert.DoesNotThrow(() => new Account(user_, credentials_));
        }

        [Test]
        public void WhenCannotCreateAccountWithoutUser()
        {
            Assert.Throws<ArgumentNullException>(() => new Account(null, credentials_));
        }

        [Test]
        public void WhenCannotCreateAccountWithoutCredentials()
        {
            Assert.Throws<ArgumentNullException>(() => new Account(user_, null));
        }

        [Test]
        public void WhenCannotCreateAccountWithoutUserAndCredentials()
        {
            Assert.Throws<ArgumentNullException>(() => new Account(null, null));
        }

        [Test]
        public void WhenAccessingAccountUser()
        {
            var account = new Account(user_, credentials_);
            Assert.AreEqual(account.User, user_);
        }

        [Test]
        public void WhenAccessingAccountCredentials()
        {
            var account = new Account(user_, credentials_);
            Assert.AreEqual(account.Credentials, credentials_);
        }

        [Test]
        public void WhenSameAccountsAreEquals()
        {
            var account1 = new Account(new User(0), new Credentials("1", "2"));
            var account2 = new Account(new User(0), new Credentials("1", "2"));
            Assert.AreEqual(account1, account2);
        }

        [Test]
        public void WhenDifferentAccountsAreNotEqualsByUser()
        {
            var account1 = new Account(new User(0), new Credentials("1", "2"));
            var account2 = new Account(new User(1), new Credentials("1", "2"));
            Assert.AreNotEqual(account1, account2);
        }

        [Test]
        public void WhenDifferentAccountsAreNotEqualsByCredentials()
        {
            var creds1 = new Credentials("1", "2");
            var creds2 = new Credentials("2", "3");
            Assert.AreNotEqual(creds1, creds2);
            var account1 = new Account(new User(0), creds1);
            var account2 = new Account(new User(0), creds2);
            Assert.AreNotEqual(account1, account2);
        }

        [Test]
        public void WhenDifferentAccountsAreNotEqualsByUserAndCredentials()
        {
            var creds1 = new Credentials("1", "2");
            var creds2 = new Credentials("2", "3");
            Assert.AreNotEqual(creds1, creds2);
            var account1 = new Account(new User(0), creds1);
            var account2 = new Account(new User(1), creds2);
            Assert.AreNotEqual(account1, account2);
        }
    }
}
