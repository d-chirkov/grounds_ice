namespace GroundsIce.Model.Validators.ProfileInfo.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GroundsIce.Model.Entities;
    using NUnit.Framework;

    [TestFixture]
    public class UniqueTypesValidatorTests
    {
        private UniqueTypesValidator subject;

        [SetUp]
        public void SetUp()
        {
            this.subject = new UniqueTypesValidator();
        }

        [Test]
        public void ValidateAsync_ThrowArgumentNullException_When_PassingNullArg()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.subject.ValidateAsync(null));
        }

        [Test]
        public async Task ValidateAsync_ReturnTrue_When_PassingEmptyList()
        {
            Assert.IsTrue(await this.subject.ValidateAsync(new List<ProfileInfoEntry>()));
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
