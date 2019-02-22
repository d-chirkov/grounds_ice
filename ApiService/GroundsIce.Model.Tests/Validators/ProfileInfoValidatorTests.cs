namespace GroundsIce.Model.Validators.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.Model.Entities;
    using NUnit.Framework;

    [TestFixture]
    public class ProfileInfoValidatorTests
    {
        private List<ProfileInfoEntry> profileInfo;
        private IProfileInfoValidator subject;

        [SetUp]
        public void SetUp()
        {
            this.profileInfo = new List<ProfileInfoEntry>
            {
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "ab", IsPublic = false },
                new ProfileInfoEntry { Type = ProfileInfoType.LastName, Value = "abc", IsPublic = false },
            };
           this.subject = new ProfileInfoValidator();
        }

        [Test]
        public void ValidateAsync_ThrowArgumentNullException_When_PassingNullProfileInfo()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.subject.ValidateAsync(null));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_PassingEmptyProfileInfo()
        {
            Assert.IsTrue(await this.subject.ValidateAsync(new List<ProfileInfoEntry>()));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_TypesMaxLengthsPropNotInitialized()
        {
            Assert.IsTrue(await this.subject.ValidateAsync(this.profileInfo));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_TypesMaxLengthsHasExactMaxLengthsAsInPassingProfileInfo()
        {
            IProfileInfoValidator validator = new ProfileInfoValidator()
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
            IProfileInfoValidator validator = new ProfileInfoValidator()
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
            IProfileInfoValidator validator = new ProfileInfoValidator()
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
            IProfileInfoValidator validator = new ProfileInfoValidator()
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
            IProfileInfoValidator validator = new ProfileInfoValidator()
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
            IProfileInfoValidator validator = new ProfileInfoValidator()
            {
                TypesMaxLengths = new Dictionary<ProfileInfoType, int>
                {
                    { ProfileInfoType.FirstName, 5 },
                    { ProfileInfoType.LastName, 6 }
                }
            };
            Assert.IsTrue(await validator.ValidateAsync(this.profileInfo));
        }

        [Test]
        public async Task ValidateAsync_ReturnFalse_When_PassingOneEmptyField()
        {
            var arg = new[]
            {
                new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = string.Empty, IsPublic = true }
            }.ToList();
            Assert.IsFalse(await this.subject.ValidateAsync(arg));
        }

        [Test]
        public async Task ValidateAsync_ReturnFalse_When_PassingTwoEmptyField()
        {
            var arg = new[]
            {
                new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = string.Empty, IsPublic = true },
                new ProfileInfoEntry() { Type = ProfileInfoType.LastName, Value = string.Empty, IsPublic = true }
            }.ToList();
            Assert.IsFalse(await this.subject.ValidateAsync(arg));
        }

        [Test]
        public async Task ValidateAsync_ReturnFalse_When_OneOfTwoPassingFieldIsEmpty()
        {
            var arg = new[]
            {
                new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = string.Empty, IsPublic = true },
                new ProfileInfoEntry() { Type = ProfileInfoType.LastName, Value = "a", IsPublic = true }
            }.ToList();
            Assert.IsFalse(await this.subject.ValidateAsync(arg));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_PassingEmptyList()
        {
            Assert.IsTrue(await this.subject.ValidateAsync(new List<ProfileInfoEntry>()));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_PassingOneNotEmptyField()
        {
            var arg = new[]
            {
                new ProfileInfoEntry() { Type = ProfileInfoType.LastName, Value = "a", IsPublic = true }
            }.ToList();
            Assert.IsTrue(await this.subject.ValidateAsync(arg));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_PassingTwoNotEmptyField()
        {
            var arg = new[]
            {
                new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = true },
                new ProfileInfoEntry() { Type = ProfileInfoType.LastName, Value = "v", IsPublic = true }
            }.ToList();
            Assert.IsTrue(await this.subject.ValidateAsync(arg));
        }

        [Test]
        public void ValidateAsync_ThrowArgumentNullException_When_PassingNullArg()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.subject.ValidateAsync(null));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_PassingUniqueTypes()
        {
            var arg = new[]
            {
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = true },
                new ProfileInfoEntry { Type = ProfileInfoType.LastName, Value = "b", IsPublic = true },
            }.ToList();
            Assert.IsTrue(await this.subject.ValidateAsync(arg));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_PassingOneEntry()
        {
            var arg = new[]
            {
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = true },
            }.ToList();
            Assert.IsTrue(await this.subject.ValidateAsync(arg));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_PassingTwoNotUniqueTypes()
        {
            var arg = new[]
            {
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = true },
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "b", IsPublic = true },
            }.ToList();
            Assert.IsFalse(await this.subject.ValidateAsync(arg));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_PassingThreeNotUniqueTypes()
        {
            var arg = new[]
            {
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = true },
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "b", IsPublic = true },
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "c", IsPublic = true },
            }.ToList();
            Assert.IsFalse(await this.subject.ValidateAsync(arg));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_PassingThreeEntriesWithTwoNotUniqueTypes()
        {
            var arg = new[]
            {
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = true },
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "b", IsPublic = true },
                new ProfileInfoEntry { Type = ProfileInfoType.LastName, Value = "c", IsPublic = true },
            }.ToList();
            Assert.IsFalse(await this.subject.ValidateAsync(arg));
        }
    }
}
