namespace GroundsIce.Model.Entities.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GroundsIce.Model.Entities;
    using NUnit.Framework;

    [TestFixture]
    public class ProfileEntriesCollectionTests
    {
        private readonly ProfileEntry profileEntry1 = new ProfileEntry { Value = "a" };
        private readonly ProfileEntry profileEntry2 = new ProfileEntry { Value = "b" };
        private List<ProfileEntry> innerCollection;
        private ProfileEntriesCollection subject;

        [SetUp]
        public void SetUp()
        {
            this.innerCollection = new List<ProfileEntry> { this.profileEntry1, this.profileEntry2 };
            this.subject = new ProfileEntriesCollection(this.innerCollection);
        }

        [Test]
        public void Ctor_DoesNotThrow_When_InvokedWithoutArgs()
        {
            Assert.DoesNotThrow(() => new ProfileEntriesCollection());
        }

        [Test]
        public void Ctor_DoesNotThrow_When_InvokedWithNotNullArg()
        {
            Assert.DoesNotThrow(() => new ProfileEntriesCollection(new List<ProfileEntry>()));
        }

        [Test]
        public void Count_ReturnZero_When_CollectionIsEmpty()
        {
            var profileEntries = new ProfileEntriesCollection(new List<ProfileEntry>());
            Assert.AreEqual(profileEntries.Count, 0);
            profileEntries = new ProfileEntriesCollection();
            Assert.AreEqual(profileEntries.Count, 0);
        }

        [Test]
        public void Count_ReturnInnerCollectionLength_When_NotEmptyInnerCollectionPassedToCtor()
        {
            Assert.AreEqual(this.subject.Count, this.innerCollection.Count);
        }

        [Test]
        public void Count_ReturnInitialLengthPlusOne_When_AddWasInvoked()
        {
            var profileEntries = new ProfileEntriesCollection(this.innerCollection.ToList());
            profileEntries.Add(new ProfileEntry());
            Assert.AreEqual(profileEntries.Count, this.innerCollection.Count + 1);
        }

        [Test]
        public void Count_ReturnInitialLengthPlusOne_When_RemoveWasInvoked()
        {
            var profileEntries = new ProfileEntriesCollection(this.innerCollection.ToList());
            profileEntries.Remove(this.innerCollection[0]);
            Assert.AreEqual(profileEntries.Count, this.innerCollection.Count - 1);
        }

        [Test]
        public void Count_ReturnZero_When_ClearWasInvoked()
        {
            this.subject.Clear();
            Assert.AreEqual(this.subject.Count, 0);
        }

        [Test]
        public void Add_PutsItemToInnerCollection()
        {
            var newItem = new ProfileEntry();
            this.subject.Add(newItem);
            Assert.IsTrue(this.innerCollection.Last() == newItem);
        }

        [Test]
        public void Remove_DeletesItemFromInnerCollection()
        {
            var deletingItem = this.innerCollection[0];
            this.subject.Remove(deletingItem);
            Assert.IsFalse(this.innerCollection.Contains(deletingItem));
        }
    }
}
