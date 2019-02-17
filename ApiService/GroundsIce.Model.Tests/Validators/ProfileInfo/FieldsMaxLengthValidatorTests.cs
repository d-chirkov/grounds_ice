namespace GroundsIce.Model.Validators.ProfileInfo.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GroundsIce.Model.Entities;
    using NUnit.Framework;

    [TestFixture]
    public class FieldsMaxLengthValidatorTests
    {
        private List<ProfileInfoEntry> profileInfo;

        [SetUp]
        public void SetUp()
        {
            this.profileInfo = new List<ProfileInfoEntry>
            {
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "ab", IsPublic = false },
                new ProfileInfoEntry { Type = ProfileInfoType.LastName, Value = "abc", IsPublic = false },
            };
        }

        [Test]
        public void ValidateAsync_ThrowArgumentNullException_When_PassingNullProfileInfo()
        {
            var validator = new FieldsMaxLengthValidator();
            Assert.ThrowsAsync<ArgumentNullException>(() => validator.ValidateAsync(null));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_PassingEmptyProfileInfo()
        {
            var validator = new FieldsMaxLengthValidator();
            Assert.IsTrue(await validator.ValidateAsync(new List<ProfileInfoEntry>()));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_TypesMaxLengthsPropNotInitialized()
        {
            var validator = new FieldsMaxLengthValidator();
            Assert.IsTrue(await validator.ValidateAsync(this.profileInfo));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_TypesMaxLengthsHasExactMaxLengthsAsInPassingProfileInfo()
        {
            var validator = new FieldsMaxLengthValidator
            {
                TypesMaxLengths = new Dictionary<ProfileInfoType, int>
                {
                    { ProfileInfoType.FirstName, 2 },
                    { ProfileInfoType.LastName, 3 }
                }
            };
            Assert.IsTrue(await validator.ValidateAsync(this.profileInfo));
        }

        [Test]
        public async Task ValidateAsync_ReturnFalse_When_OneOfValuesLengthIsNotValid()
        {
            var validator = new FieldsMaxLengthValidator
            {
                TypesMaxLengths = new Dictionary<ProfileInfoType, int>
                {
                    { ProfileInfoType.FirstName, 1 }
                }
            };
            Assert.IsFalse(await validator.ValidateAsync(this.profileInfo));
        }

        [Test]
        public async Task ValidateAsync_ReturnFalse_When_AllValuesLengthsAreNotValid()
        {
            var validator = new FieldsMaxLengthValidator
            {
                TypesMaxLengths = new Dictionary<ProfileInfoType, int>
                {
                    { ProfileInfoType.FirstName, 1 },
                    { ProfileInfoType.LastName, 1 }
                }
            };
            Assert.IsFalse(await validator.ValidateAsync(this.profileInfo));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_TypesMaxLengthsHasMoreLengthThenPassingProfileInfo()
        {
            var validator = new FieldsMaxLengthValidator
            {
                TypesMaxLengths = new Dictionary<ProfileInfoType, int>
                {
                    { ProfileInfoType.FirstName, 2 },
                    { ProfileInfoType.LastName, 3 },
                    { ProfileInfoType.MiddleName, 1 }
                }
            };
            Assert.IsTrue(await validator.ValidateAsync(this.profileInfo));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_TypesMaxLengthsHasLessLengthThenPassingProfileInfo()
        {
            var validator = new FieldsMaxLengthValidator
            {
                TypesMaxLengths = new Dictionary<ProfileInfoType, int>
                {
                    { ProfileInfoType.FirstName, 2 }
                }
            };
            Assert.IsTrue(await validator.ValidateAsync(this.profileInfo));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_TypesMaxLengthsHasGreaterMaxLengthThenInPassingProfileInfo()
        {
            var validator = new FieldsMaxLengthValidator
            {
                TypesMaxLengths = new Dictionary<ProfileInfoType, int>
                {
                    { ProfileInfoType.FirstName, 5 },
                    { ProfileInfoType.LastName, 6 }
                }
            };
            Assert.IsTrue(await validator.ValidateAsync(this.profileInfo));
        }
    }
}
