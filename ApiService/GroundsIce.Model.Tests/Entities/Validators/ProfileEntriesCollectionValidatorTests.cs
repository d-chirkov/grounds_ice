namespace GroundsIce.Model.Entities.Validators.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.Model.Entities;
    using NUnit.Framework;

    [TestFixture]
    public class ProfileEntriesCollectionValidatorTests
    {
        public ValidationsCollection<ProfileEntriesCollection> Validations { get; } = new ValidationsCollection<ProfileEntriesCollection>
        {
            new Validation<ProfileEntriesCollection>
            {
                Description = "Non empty values should be valid with null TypesMaxLengths",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = true,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Empty entries collection should be valid",
                Object = new ProfileEntriesCollection(),
                Validator = new ProfileEntriesCollectionValidator(),
                Result = true,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection of two should be invalid, if it contains one null value",
                Object = new ProfileEntriesCollection()
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = null, IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "ab", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = false,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection of one should be invalid, if it contains only one null value",
                Object = new ProfileEntriesCollection()
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = null, IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = false,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection of two should be invalid, if it contains two null values",
                Object = new ProfileEntriesCollection()
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = null, IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = null, IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = false,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection of two should be invalid, if it contains one empty value",
                Object = new ProfileEntriesCollection()
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = string.Empty, IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = false,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection of one should be invalid, if it contains only one empty value",
                Object = new ProfileEntriesCollection()
                {
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = string.Empty, IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = false,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection of two should be invalid, if it contains two empty values",
                Object = new ProfileEntriesCollection()
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = string.Empty, IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = string.Empty, IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = false,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection should be valid, when entries has exact length as in TypesMaxLengths",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator
                {
                    TypesMaxLengths = new Dictionary<ProfileEntryType, int>
                    {
                        { ProfileEntryType.FirstName, 2 },
                        { ProfileEntryType.LastName, 3 }
                    }
                },
                Result = true,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection should be invalid, when one of entries value length is not allowed",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator
                {
                    TypesMaxLengths = new Dictionary<ProfileEntryType, int>
                    {
                        { ProfileEntryType.FirstName, 1 }
                    }
                },
                Result = false,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection should be invalid, when all entries value lengths are not allowed",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator
                {
                    TypesMaxLengths = new Dictionary<ProfileEntryType, int>
                    {
                        { ProfileEntryType.FirstName, 1 },
                        { ProfileEntryType.LastName, 2 }
                    }
                },
                Result = false,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection should be valid, when TypesMaxLengths has more lengths then passing collection",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator
                {
                    TypesMaxLengths = new Dictionary<ProfileEntryType, int>
                    {
                        { ProfileEntryType.FirstName, 2 },
                        { ProfileEntryType.LastName, 3 },
                        { ProfileEntryType.MiddleName, 1 }
                    }
                },
                Result = true,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection should be valid, when TypesMaxLengths has less lengths then passing collection",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator
                {
                    TypesMaxLengths = new Dictionary<ProfileEntryType, int>
                    {
                        { ProfileEntryType.FirstName, 2 },
                    }
                },
                Result = true,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection should be valid, when TypesMaxLengths has greater max length then in passing collection",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator
                {
                    TypesMaxLengths = new Dictionary<ProfileEntryType, int>
                    {
                        { ProfileEntryType.FirstName, 5 },
                        { ProfileEntryType.LastName, 6 }
                    }
                },
                Result = true,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection should be valid, when TypesMaxLengths has greater by one max length then in passing collection",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator
                {
                    TypesMaxLengths = new Dictionary<ProfileEntryType, int>
                    {
                        { ProfileEntryType.FirstName, 3 },
                        { ProfileEntryType.LastName, 4 }
                    }
                },
                Result = true,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection should be valid, when it has only unique enties types",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = true,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection should be invalid, when it repeating enties types",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "abc", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = false,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection of two should be invalid, when it contains entries of one type",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = false,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection of three should be invalid, when it contains entries of one type",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "abc", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "abc", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = false,
            },

            new Validation<ProfileEntriesCollection>
            {
                Description = "Full collection should be valid, when it contains only private fields",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.MiddleName, Value = "abcd", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.Region, Value = "abce", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.City, Value = "abcef", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.Description, Value = "abcefg", IsPublic = false },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = true,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Full collection should be valid, when it contains public and not public fields",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.MiddleName, Value = "abcd", IsPublic = true },
                    new ProfileEntry { Type = ProfileEntryType.Region, Value = "abce", IsPublic = true },
                    new ProfileEntry { Type = ProfileEntryType.City, Value = "abcef", IsPublic = false },
                    new ProfileEntry { Type = ProfileEntryType.Description, Value = "abcefg", IsPublic = true },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = true,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Full collection should be valid, when it contains only public fields",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "ab", IsPublic = true },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = true },
                    new ProfileEntry { Type = ProfileEntryType.MiddleName, Value = "abcd", IsPublic = true },
                    new ProfileEntry { Type = ProfileEntryType.Region, Value = "abce", IsPublic = true },
                    new ProfileEntry { Type = ProfileEntryType.City, Value = "abcef", IsPublic = true },
                    new ProfileEntry { Type = ProfileEntryType.Description, Value = "abcefg", IsPublic = true },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = true,
            },
            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection of one should be invalid, when it entry has not existing Type (not in enum)",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry
                    {
                        Type = Enum.GetValues(typeof(ProfileEntryType)).Cast<ProfileEntryType>().Max() + 1,
                        Value = "ab",
                        IsPublic = true
                    }
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = false,
            },

            new Validation<ProfileEntriesCollection>
            {
                Description = "Collection of two should be invalid, when one of its entries has not existing Type (not in enum)",
                Object = new ProfileEntriesCollection
                {
                    new ProfileEntry
                    {
                        Type = Enum.GetValues(typeof(ProfileEntryType)).Cast<ProfileEntryType>().Max() + 1,
                        Value = "ab",
                        IsPublic = true
                    },
                    new ProfileEntry { Type = ProfileEntryType.LastName, Value = "abc", IsPublic = true },
                },
                Validator = new ProfileEntriesCollectionValidator(),
                Result = false,
            },
        };

        [Test]
        public void ValidateAsync_ThrowArgumentNullException_When_PassingNullProfileInfo()
        {
            var validator = new ProfileEntriesCollectionValidator();
            Assert.Throws<ArgumentNullException>(() => validator.Validate(null as ProfileEntriesCollection));
        }

        [Test]
        public void Validate_ReturnTrue_When_PassingEmptyProfileInfo()
        {
            var validator = new ProfileEntriesCollectionValidator();
            Assert.IsTrue(validator.Validate(new ProfileEntriesCollection()).IsValid);
        }

        [Test]
        public void Validate_ReturnExpextedValue_InAllCases()
        {
            this.Validations.AssertAll();
        }
    }
}
