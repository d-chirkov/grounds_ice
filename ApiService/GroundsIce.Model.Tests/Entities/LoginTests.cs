namespace GroundsIce.Model.Entities.Tests
{
    using System;
    using GroundsIce.Model.Entities;
    using NUnit.Framework;

    [TestFixture]
    public class LoginTests
    {
        [Test]
        public void Ctor_ThrowArgumentNullException_When_PassingNullValue()
        {
            Assert.Throws<ArgumentNullException>(() => new Login(null));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingNotNullButEmptyValue()
        {
            Assert.DoesNotThrow(() => new Login(string.Empty));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingNotEmptyValue()
        {
            Assert.DoesNotThrow(() => new Login("a"));
        }

        [Test]
        public void Equals_ReturnFalse_When_LoginsHasDifferentValues()
        {
            var login1 = new Login("a");
            var login2 = new Login("b");
            Assert.AreNotEqual(login1, login2);
        }

        [Test]
        public void Equals_ReturnTrue_When_LoginsHasDifferentValues()
        {
            var login1 = new Login("a");
            var login2 = new Login("a");
            Assert.AreEqual(login1, login2);
        }

        [Test]
        public void ValueGetter_ReturnTheSameValueAsInCtorArg_When_CallGetter()
        {
            string value = "a";
            var login = new Login(value);
            Assert.AreEqual(login.Value, value);
        }

        [Test]
        public void ValueSetter_ChangeActualValue_When_CallSetter()
        {
            string value = "a";
            var login = new Login(string.Empty);
            login.Value = value;
            Assert.AreEqual(login.Value, value);
        }

        [Test]
        public void ValueSetter_ThrowArgumentNullException_When_CallSetterWithNullArg()
        {
            var login = new Login(string.Empty);
            Assert.Throws<ArgumentNullException>(() => login.Value = null);
        }

        [Test]
        public void ValueSetter_DoesNotThrow_When_CallSetterWithEmptyArg()
        {
            var login = new Login("a");
            Assert.DoesNotThrow(() => login.Value = string.Empty);
        }
    }
}
