using System;
using NUnit.Framework;
using GroundsIce.Model.Accounting;

namespace GroundsIce.Accounting.CredentialsValidators.Tests
{
    [TestFixture]
    public class NameLengthValidatorTests
    {
        [Test]
        public void WhenCreating()
        {
            Assert.DoesNotThrow(() => new NameLengthValidator(0, 0));
        }

        [Test]
        public void WhenCannotCreateWithMinLengthGreaterThenMax()
        {
            Assert.Throws<ArgumentException>(() => new NameLengthValidator(1, 0));
        }

        [Test]
        public void WhenCannotCreateWithNegativeArgs()
        {
            Assert.Throws<ArgumentException>(() => new NameLengthValidator(-1, 0));
            Assert.Throws<ArgumentException>(() => new NameLengthValidator(0, -1));
            Assert.Throws<ArgumentException>(() => new NameLengthValidator(-1, -1));
        }

        [Test]
        public void WhenSuccessedValidatingNameLength()
        {
            string username = "user";
            var validator = new NameLengthValidator(username.Length, username.Length);
            Assert.DoesNotThrow(() => Assert.True(validator.Validate(username)));
        }

        [Test]
        public void WhenFailedValidatingTooShortNameLength()
        {
            string username = "user";
            var validator = new NameLengthValidator(username.Length + 1, username.Length + 2);
            Assert.False(validator.Validate(username));
        }

        [Test]
        public void WhenFailedValidatingTooLongNameLength()
        {
            string username = "user";
            var validator = new NameLengthValidator(username.Length - 2, username.Length - 1);
            Assert.False(validator.Validate(username));
        }

        [Test]
        public void WhenFailedValidatingEmptyNameLength()
        {
            var validator = new NameLengthValidator(1, 2);
            Assert.False(validator.Validate(""));
        }
    }
}
