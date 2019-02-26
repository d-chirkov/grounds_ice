namespace GroundsIce.Model.Entities.Validators.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    public class LoginValidatorTests
    {
        public List<LoginValueValidation> Validations { get; } = new List<LoginValueValidation>
        {
            new LoginValueValidation { Login = new Login("a"), Validator = new LoginValidator(1, 2),  Result = true },
            new LoginValueValidation { Login = new Login("ab"), Validator = new LoginValidator(1, 2),  Result = true },
            new LoginValueValidation { Login = new Login("ab"), Validator = new LoginValidator(2, 4),  Result = true },
            new LoginValueValidation { Login = new Login("abcd"), Validator = new LoginValidator(2, 4),  Result = true },
            new LoginValueValidation { Login = new Login("abc"), Validator = new LoginValidator(2, 4),  Result = true },
            new LoginValueValidation { Login = new Login("a"), Validator = new LoginValidator(2, 4),  Result = false },
            new LoginValueValidation { Login = new Login("a"), Validator = new LoginValidator(2, 3),  Result = false },
            new LoginValueValidation { Login = new Login("abc"), Validator = new LoginValidator(1, 2),  Result = false },
            new LoginValueValidation { Login = new Login("abcde"), Validator = new LoginValidator(2, 4),  Result = false },
            new LoginValueValidation { Login = new Login("abcdef"), Validator = new LoginValidator(2, 4),  Result = false },
            new LoginValueValidation { Login = new Login(string.Empty), Validator = new LoginValidator(1, 2),  Result = false },
            new LoginValueValidation { Login = new Login(string.Empty), Validator = new LoginValidator(2, 3),  Result = false },
        };

        [Test]
        public void Ctor_ThrowArgumentOutOfRangeException_When_PassingNegativeMinLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LoginValidator(-1, 1));
        }

        [Test]
        public void Ctor_ThrowArgumentOutOfRangeException_When_PassingZeroMinLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LoginValidator(0, 1));
        }

        [Test]
        public void Ctor_ThrowArgumentOutOfRangeException_When_PassingMinLengthIsGreaterThenMaxLength()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LoginValidator(2, 1));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingMinLengthIsEqualsMaxLength()
        {
            Assert.DoesNotThrow(() => new LoginValidator(1, 1));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingMinLengthIsLessThenMaxLength()
        {
            Assert.DoesNotThrow(() => new LoginValidator(1, 2));
        }

        [Test]
        public void Validate_ReturnExpextedValue_InAllCases()
        {
            for (int i = 0; i < this.Validations.Count; ++i)
            {
                var validation = this.Validations[i];
                Assert.AreEqual(validation.Result, validation.Validator.Validate(validation.Login).IsValid, i.ToString());
            }
        }

        public class LoginValueValidation
        {
            public Login Login { get; set; }

            public LoginValidator Validator { get; set; }

            public bool Result { get; set; }

            public string FailedPassingDescription { get; set; }
        }
    }
}
