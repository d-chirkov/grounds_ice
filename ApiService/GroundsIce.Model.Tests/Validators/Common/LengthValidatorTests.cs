namespace GroundsIce.Model.Validators.Common.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class LengthValidatorTests
    {
        [Test]
        public void Ctor_ThrowArgumentOutOfRangeException_When_MinLengthIsLessOrEqualsZero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LengthValidator(-1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LengthValidator(0, 1));
        }

        [Test]
        public void Ctor_ThrowArgumentOutOfRangeException_When_MaxLengthIsLessThenMinLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LengthValidator(1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LengthValidator(2, 1));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_MaxLengthIsEqualsMinLength()
        {
            Assert.DoesNotThrow(() => new LengthValidator(1, 1));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_MaxLengthIsGreaterThenMinLength()
        {
            Assert.DoesNotThrow(() => new LengthValidator(1, 2));
        }

        [Test]
        public async Task Validate_ReturnsTrue_When_ValueLengthIsEqualsMinLength()
        {
            string value = "a";
            var validator = new LengthValidator(value.Length, value.Length + 1);
            Assert.IsTrue(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsTrue_When_ValueLengthIsEqualsMaxLength()
        {
            string value = "ab";
            var validator = new LengthValidator(value.Length - 1, value.Length);
            Assert.IsTrue(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsTrue_When_ValueLengthIsBetweenMinAndMaxLength()
        {
            string value = "ab";
            var validator = new LengthValidator(value.Length - 1, value.Length + 1);
            Assert.IsTrue(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsTrue_When_ValueLengthIsEqualsMinAndMaxLength()
        {
            string value = "a";
            var validator = new LengthValidator(value.Length, value.Length);
            Assert.IsTrue(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsFalse_When_ValueLengthIsLessThenMinLength()
        {
            string value = "a";
            var validator = new LengthValidator(value.Length + 1, value.Length + 2);
            Assert.IsFalse(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsFalse_When_ValueLengthIsGreaterThenMaxLength()
        {
            string value = "abc";
            var validator = new LengthValidator(value.Length - 2, value.Length - 1);
            Assert.IsFalse(await validator.ValidateAsync(value));
        }

        [Test]
        public async Task Validate_ReturnsFalse_When_ValueIsNull()
        {
            var validator = new LengthValidator(1, 2);
            Assert.IsFalse(await validator.ValidateAsync(null));
        }

        [Test]
        public async Task Validate_ReturnsFalse_When_ValueIsEmpty()
        {
            var validator = new LengthValidator(1, 2);
            Assert.IsFalse(await validator.ValidateAsync(string.Empty));
        }
    }
}
