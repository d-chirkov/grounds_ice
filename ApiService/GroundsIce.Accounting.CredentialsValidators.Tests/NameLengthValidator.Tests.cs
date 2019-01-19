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
            var credentials = new Credentials("user", "");
            var validator = new NameLengthValidator(credentials.Name.Length, credentials.Name.Length);
            Assert.DoesNotThrow(() => validator.Validate(credentials));
        }

        [Test]
        public void WhenFailedValidatingTooShortNameLength()
        {
            var credentials = new Credentials("user", "");
            var validator = new NameLengthValidator(credentials.Name.Length + 1, credentials.Name.Length + 2);
            Assert.Throws<CredentialsValidatorException>(() => validator.Validate(credentials));
        }

        [Test]
        public void WhenFailedValidatingTooLongNameLength()
        {
            var credentials = new Credentials("user", "");
            var validator = new NameLengthValidator(credentials.Name.Length - 2, credentials.Name.Length - 1);
            Assert.Throws<CredentialsValidatorException>(() => validator.Validate(credentials));
        }

        [Test]
        public void WhenFailedValidatingEmptyNameLength()
        {
            var credentials = new Credentials("", "");
            var validator = new NameLengthValidator(1, 2);
            Assert.Throws<CredentialsValidatorException>(() => validator.Validate(credentials));
        }
    }
}
