﻿using System;
using NUnit.Framework;

namespace GroundsIce.Model.Entities.Tests
{
    [TestFixture]
    class AccountTests
    {
        private long _userId;
        [SetUp]
        public void SetUp()
        {
            _userId = 0;
        }

        [Test]
        public void Ctor_ThrowArgumentOutOfRangeException_When_UserIdIsLessThenZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Account(-1, ""));
        }

        [Test]
        public void Ctor_ThrowArgumentNullException_When_LoginIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Account(_userId, null));
        }
		
        [Test]
        public void Ctor_Create_When_LoginAndUserIdAreValid()
        {
            Assert.DoesNotThrow(() => new Account(_userId, "a"));
        }

        [Test]
        public void Ctor_SaveUserAndLoginAsProperties_When_LoginAndUserIdAreValid()
        {
            string login = "a";
            var account = new Account(_userId, login);
            Assert.AreEqual(_userId, account.UserId);
            Assert.AreEqual(login, account.Login);
        }
    }
}