namespace GroundsIce.Model.Validators.Tests
{
    using System;
    using System.Threading.Tasks;
    using GroundsIce.Model.Abstractions.Validators;
    using NUnit.Framework;

    [TestFixture]
    public class PasswordValidatorTests
    {
        [Test]
        public void Ctor_ThrowArgumentOutOfRangeException_When_MinLengthIsLessOrEqualsZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PasswordValidator(-1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PasswordValidator(0, 1));
        }

        [Test]
        public void Ctor_ThrowArgumentOutOfRangeException_When_MaxLengthIsLessThenMinLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new PasswordValidator(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PasswordValidator(2, 1));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_MaxLengthIsEqualsMinLength()
        {
            Assert.DoesNotThrow(() => new PasswordValidator(1, 1));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_MaxLengthIsGreaterThenMinLength()
        {
            Assert.DoesNotThrow(() => new PasswordValidator(1, 2));
        }

        [Test]
        public async Task Validate_ReturnsTrue_When_ValueLengthIsEqualsMinLength()
        {
            string value = "a";
            IStringValidator validator = new PasswordValidator(value.Length, value.Length + 1);
            Assert.IsTrue(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsTrue_When_ValueLengthIsEqualsMaxLength()
        {
            string value = "ab";
            IStringValidator validator = new PasswordValidator(value.Length - 1, value.Length);
            Assert.IsTrue(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsTrue_When_ValueLengthIsBetweenMinAndMaxLength()
        {
            string value = "ab";
            IStringValidator validator = new PasswordValidator(value.Length - 1, value.Length + 1);
            Assert.IsTrue(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsTrue_When_ValueLengthIsEqualsMinAndMaxLength()
        {
            string value = "a";
            IStringValidator validator = new PasswordValidator(value.Length, value.Length);
            Assert.IsTrue(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsFalse_When_ValueLengthIsLessThenMinLength()
        {
            string value = "a";
            IStringValidator validator = new PasswordValidator(value.Length + 1, value.Length + 2);
            Assert.IsFalse(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsFalse_When_ValueLengthIsGreaterThenMaxLength()
        {
            string value = "abc";
            IStringValidator validator = new PasswordValidator(value.Length - 2, value.Length - 1);
            Assert.IsFalse(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsFalse_When_ValueIsNull()
        {
            IStringValidator validator = new PasswordValidator(1, 2);
            Assert.IsFalse(await validator.ValidateAsync(null));
        }

        [Test]
        public async Task Validate_ReturnsFalse_When_ValueIsEmpty()
        {
            IStringValidator validator = new PasswordValidator(1, 2);
            Assert.IsFalse(await validator.ValidateAsync(string.Empty));
        }
    }
}
