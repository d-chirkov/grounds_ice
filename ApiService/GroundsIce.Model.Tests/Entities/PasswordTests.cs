namespace GroundsIce.Model.Entities.Tests
{
    using System;
    using GroundsIce.Model.Entities;
    using NUnit.Framework;

    [TestFixture]
    public class PasswordTests
    {
        [Test]
        public void Ctor_ThrowArgumentNullException_When_PassingNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => new Password(null));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingNotNullButEmptyValue()
        {
            Assert.DoesNotThrow(() => new Password(string.Empty));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingNotEmptyValue()
        {
            Assert.DoesNotThrow(() => new Password("a"));
        }

        [Test]
        public void Equals_ReturnFalse_When_PasswordsHasDifferentValues()
        {
            var password1 = new Password("a");
            var password2 = new Password("b");
            Assert.AreNotEqual(password1, password2);
        }

        [Test]
        public void Equals_ReturnTrue_When_PasswordsHasDifferentValues()
        {
            var password1 = new Password("a");
            var password2 = new Password("a");
            Assert.AreEqual(password1, password2);
        }

        [Test]
        public void ValueGetter_ReturnTheSameValueAsInCtorArg_When_CallGetter()
        {
            string value = "a";
            var password = new Password(value);
            Assert.AreEqual(password.Value, value);
        }
    }
}
