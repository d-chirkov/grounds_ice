namespace GroundsIce.Model.Repositories.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using GroundsIce.Model.Abstractions;
    using GroundsIce.Model.ConnectionFactories;
    using GroundsIce.Model.Entities;
    using NUnit.Framework;

    [TestFixture]
    public class DbProfileRepositoryTests
    {
        private const string AccountsTableName = "Accounts";
        private readonly Login validLogin = new Login("login");
        private readonly Password validPassword = new Password("password");
        private readonly Login anotherValidLogin = new Login("loginn");
        private readonly Password anotherValidPassword = new Password("passwordd");
        private IConnectionFactory connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");

        private DbProfileRepository subject;
        private DbAccountRepository accountRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");
        }

        [SetUp]
        public async Task SetUp()
        {
            this.accountRepository = new DbAccountRepository(this.connectionFactory);
            this.subject = new DbProfileRepository(this.connectionFactory);
            using (var connection = await this.connectionFactory.GetConnectionAsync())
            {
                await connection.ExecuteAsync($"DELETE FROM {AccountsTableName}");
            }
        }

        [Test]
        public void Ctor_Throw_When_PassingNullConnectionFactory()
        {
            Assert.Throws<ArgumentNullException>(() => new DbProfileRepository(null));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingValidConnectionFactory()
        {
            Assert.DoesNotThrow(() => new DbProfileRepository(this.connectionFactory));
        }

        [Test]
        public void GetProfileAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.subject.GetProfileAsync(-1));
        }

        [Test]
        public async Task GetProfileAsync_ReturnNullProfile_When_PassingNotExistingUserId()
        {
            Account account = await this.accountRepository.CreateAccountAsync(this.validLogin, this.validPassword);
            Profile profile = await this.subject.GetProfileAsync(account.UserId + 1);
            Assert.IsNull(profile);
        }

        [Test]
        public async Task GetProfileAsync_ReturnNotNullProfile_When_PassingExistingUserId()
        {
            Account account = await this.accountRepository.CreateAccountAsync(this.validLogin, this.validPassword);
            Profile profile = await this.subject.GetProfileAsync(account.UserId);
            Assert.IsNotNull(profile);
            Assert.AreEqual(profile.Login, this.validLogin);
        }

        [Test]
        public void SetProfileInfoAsync_ThrowArgumentNullException_When_PassingNullProfileInfo()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.subject.SetProfileEntriesCollectionAsync(1, null));
        }

        [Test]
        public void SetProfileInfoAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            this.subject.SetProfileEntriesCollectionAsync(-1, new ProfileEntriesCollection()));
        }

        [Test]
        public void SetProfileInfoAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZeroAndNullProfileInfo()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.subject.SetProfileEntriesCollectionAsync(-1, null));
        }

        [Test]
        public async Task SetProfileInfoAsync_ReturnFalse_When_PassingUserIdNotExistsWithEmptyProfileInfo()
        {
            bool updated = await this.subject.SetProfileEntriesCollectionAsync(1, new ProfileEntriesCollection());
            Assert.IsFalse(updated);
        }

        [Test]
        public async Task SetProfileInfoAsync_ReturnFalse_When_PassingUserIdNotExistsWithNotEmptyProfileInfo()
        {
            var profileInfo = new ProfileEntriesCollection
            {
                new ProfileEntry() { Type = ProfileEntryType.FirstName, Value = "a", IsPublic = true }
            };
            bool updated = await this.subject.SetProfileEntriesCollectionAsync(1, profileInfo);
            Assert.IsFalse(updated);
        }

        [Test]
        public async Task SetProfileInfoAsync_ReturnTrue_When_PassingEmptyProfileInfo()
        {
            Account account = await this.accountRepository.CreateAccountAsync(this.validLogin, this.validPassword);
            bool updated = await this.subject.SetProfileEntriesCollectionAsync(account.UserId, new ProfileEntriesCollection());
            Assert.IsTrue(updated);
        }

        [Test]
        public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingEmptyProfileInfo()
        {
            Account account = await this.accountRepository.CreateAccountAsync(this.validLogin, this.validPassword);
            await this.subject.SetProfileEntriesCollectionAsync(account.UserId, new ProfileEntriesCollection());
            Profile profile = await this.subject.GetProfileAsync(account.UserId);
            Assert.AreEqual(profile.ProfileEntriesCollection.Count(), 0);
        }

        public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(long userId, ProfileEntriesCollection profileEntries)
        {
            await this.subject.SetProfileEntriesCollectionAsync(userId, profileEntries);
            Profile profile = await this.subject.GetProfileAsync(userId);
            Assert.AreEqual(profile.Login, this.validLogin);
            Assert.AreEqual(profile.ProfileEntriesCollection.Count(), profileEntries.Count());
            foreach (var entry in profile.ProfileEntriesCollection)
            {
                Assert.AreEqual(profileEntries.Where(v => v.Equals(entry)).Count(), 1);
            }
        }

        [Test]
        public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo()
        {
            Account account = await this.accountRepository.CreateAccountAsync(this.validLogin, this.validPassword);
            foreach (var typeName in Enum.GetNames(typeof(ProfileEntryType)))
            {
                var type = (ProfileEntryType)Enum.Parse(typeof(ProfileEntryType), typeName);
                await this.SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
                    account.UserId,
                    new ProfileEntriesCollection { new ProfileEntry() { Type = type, Value = "a", IsPublic = true } });
                await this.SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
                    account.UserId,
                    new ProfileEntriesCollection { new ProfileEntry() { Type = type, Value = "b", IsPublic = false } });
            }
        }

        [Test]
        public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingFullProfileInfo()
        {
            Account account = await this.accountRepository.CreateAccountAsync(this.validLogin, this.validPassword);
            var profileEntries = new ProfileEntriesCollection
            {
                new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "a", IsPublic = false },
                new ProfileEntry { Type = ProfileEntryType.LastName, Value = "b", IsPublic = true },
                new ProfileEntry { Type = ProfileEntryType.MiddleName, Value = "c", IsPublic = false },
                new ProfileEntry { Type = ProfileEntryType.Description, Value = "d", IsPublic = true },
                new ProfileEntry { Type = ProfileEntryType.City, Value = "e", IsPublic = false },
                new ProfileEntry { Type = ProfileEntryType.Region, Value = "f", IsPublic = false },
            };
            await this.SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
                account.UserId,
                profileEntries);
        }

        [Test]
        public async Task SetProfileInfoAsync_NotAffectGetProfileAsync_When_PassingProfileInfoWithNullOrEmptyValue()
        {
            Account account = await this.accountRepository.CreateAccountAsync(this.validLogin, this.validPassword);
            await this.subject.SetProfileEntriesCollectionAsync(
                account.UserId,
                new ProfileEntriesCollection { new ProfileEntry { Value = null } });
            await this.subject.SetProfileEntriesCollectionAsync(
                account.UserId,
                new ProfileEntriesCollection { new ProfileEntry { Value = string.Empty } });
            Profile profile = await this.subject.GetProfileAsync(account.UserId);
            Assert.AreEqual(profile.ProfileEntriesCollection.Count(), 0);
        }

        [Test]
        public async Task SetProfileInfoAsync_NotAffectGetProfileAsync_For_OtherUsers()
        {
            Account account1 = await this.accountRepository.CreateAccountAsync(this.validLogin, this.validPassword);
            Account account2 = await this.accountRepository.CreateAccountAsync(this.anotherValidLogin, this.anotherValidPassword);
            var profileEntries1 = new ProfileEntriesCollection
            {
                new ProfileEntry { Type = ProfileEntryType.FirstName, Value = "a", IsPublic = false },
                new ProfileEntry { Type = ProfileEntryType.LastName, Value = "b", IsPublic = true },
                new ProfileEntry { Type = ProfileEntryType.MiddleName, Value = "c", IsPublic = false },
            };
            var profileEntries2 = new ProfileEntriesCollection
            {
                new ProfileEntry { Type = ProfileEntryType.MiddleName, Value = "c", IsPublic = false },
                new ProfileEntry { Type = ProfileEntryType.City, Value = "d", IsPublic = true },
                new ProfileEntry { Type = ProfileEntryType.Description, Value = "e", IsPublic = false },
            };
            await this.subject.SetProfileEntriesCollectionAsync(account1.UserId, profileEntries1);
            await this.subject.SetProfileEntriesCollectionAsync(account2.UserId, profileEntries2);
            Profile profile1 = await this.subject.GetProfileAsync(account1.UserId);
            Assert.AreEqual(profile1.Login, this.validLogin);
            Assert.AreEqual(profile1.ProfileEntriesCollection.Count(), profileEntries1.Count());
            foreach (var entry in profile1.ProfileEntriesCollection)
            {
                Assert.AreEqual(profileEntries1.Where(v => v.Equals(entry)).Count(), 1);
            }

            Profile profile2 = await this.subject.GetProfileAsync(account2.UserId);
            Assert.AreEqual(profile2.Login, this.anotherValidLogin);
            Assert.AreEqual(profile2.ProfileEntriesCollection.Count(), profileEntries1.Count());
            foreach (var entry in profile2.ProfileEntriesCollection)
            {
                Assert.AreEqual(profileEntries2.Where(v => v.Equals(entry)).Count(), 1);
            }

            await this.subject.SetProfileEntriesCollectionAsync(account2.UserId, new ProfileEntriesCollection());
            profile1 = await this.subject.GetProfileAsync(account1.UserId);
            Assert.AreEqual(profile1.Login, this.validLogin);
            Assert.AreEqual(profile1.ProfileEntriesCollection.Count(), profileEntries1.Count());
            foreach (var entry in profile1.ProfileEntriesCollection)
            {
                Assert.AreEqual(profileEntries1.Where(v => v.Equals(entry)).Count(), 1);
            }

            profile2 = await this.subject.GetProfileAsync(account2.UserId);
            Assert.AreEqual(profile2.Login, this.anotherValidLogin);
            Assert.AreEqual(profile2.ProfileEntriesCollection.Count(), 0);
        }
    }
}
