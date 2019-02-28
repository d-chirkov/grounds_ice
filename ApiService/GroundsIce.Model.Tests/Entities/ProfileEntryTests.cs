namespace GroundsIce.Model.Entities.Tests
{
    using GroundsIce.Model.Entities;
    using NUnit.Framework;

    [TestFixture]
    public class ProfileEntryTests
    {
        [Test]
        public void Ctor_DoesNotThrow_When_Invoked()
        {
            Assert.DoesNotThrow(() => new ProfileEntry());
        }

        [Test]
        public void Equals_ReturnFalse_When_ComparingWithNull()
        {
            var entry = new ProfileEntry();
            Assert.AreNotEqual(entry, null);
        }

        [Test]
        public void Equals_ReturnTrue_When_ComparingObjectsWithAllDefaultFields()
        {
            var entry1 = new ProfileEntry();
            var entry2 = new ProfileEntry();
            Assert.AreEqual(entry1, entry2);
        }

        [Test]
        public void Equals_ReturnFalse_When_ComparingObjectsWithDifferentTypes()
        {
            var entry1 = new ProfileEntry { Type = ProfileEntryType.FirstName };
            var entry2 = new ProfileEntry { Type = ProfileEntryType.LastName };
            Assert.AreNotEqual(entry1, entry2);
        }

        [Test]
        public void Equals_ReturnTrue_When_ComparingObjectsWithSameTypesAndOtherFieldsAreDefault()
        {
            var entry1 = new ProfileEntry { Type = ProfileEntryType.FirstName };
            var entry2 = new ProfileEntry { Type = ProfileEntryType.FirstName };
            Assert.AreEqual(entry1, entry2);
        }

        [Test]
        public void Equals_ReturnFalse_When_ComparingObjectsWithDifferentValue()
        {
            var entry1 = new ProfileEntry { Value = "a" };
            var entry2 = new ProfileEntry { Value = "b" };
            Assert.AreNotEqual(entry1, entry2);
        }

        [Test]
        public void Equals_ReturnFalse_When_OneOfObjectsHasNullValue()
        {
            var entry1 = new ProfileEntry { Value = "a" };
            var entry2 = new ProfileEntry { Value = null };
            Assert.AreNotEqual(entry1, entry2);
        }

        [Test]
        public void Equals_ReturnTrue_When_BothObjectsHasNullValueAndOtherFieldsAreDefault()
        {
            var entry1 = new ProfileEntry { Value = null };
            var entry2 = new ProfileEntry { Value = null };
            Assert.AreEqual(entry1, entry2);
        }

        [Test]
        public void Equals_ReturnTrue_When_BothObjectsHasSameValueAndOtherFieldsAreDefault()
        {
            var entry1 = new ProfileEntry { Value = "a" };
            var entry2 = new ProfileEntry { Value = "a" };
            Assert.AreEqual(entry1, entry2);
        }

        [Test]
        public void Equals_ReturnFalse_When_ComparingObjectsWithDifferentPublicFlags()
        {
            var entry1 = new ProfileEntry { IsPublic = false };
            var entry2 = new ProfileEntry { IsPublic = true };
            Assert.AreNotEqual(entry1, entry2);
        }

        [Test]
        public void Equals_ReturnTrye_When_ComparingObjectsWithSamePublicFlagsAndOtherFieldsAreDefault()
        {
            var entry1 = new ProfileEntry { IsPublic = true };
            var entry2 = new ProfileEntry { IsPublic = true };
            Assert.AreEqual(entry1, entry2);
            entry1 = new ProfileEntry { IsPublic = false };
            entry2 = new ProfileEntry { IsPublic = false };
            Assert.AreEqual(entry1, entry2);
        }

        [Test]
        public void Equals_ReturnTrue_When_ComparingObjectsWithAllEqualsFields()
        {
            var entry1 = new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "a", IsPublic = true };
            var entry2 = new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "a", IsPublic = true };
            Assert.AreEqual(entry1, entry2);
        }
    }
}
