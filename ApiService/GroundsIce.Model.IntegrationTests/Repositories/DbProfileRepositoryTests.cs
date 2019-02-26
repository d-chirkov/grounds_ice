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
        private const string ValidLogin = "login";
        private const string ValidPassword = "password";
        private const string AnotherValidLogin = "loginn";
        private const string AnotherValidPassword = "passwordd";
        private IConnectionFactory connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");

        private DbProfileRepository subject;
        private DbAccountRepository_OLD accountRepository;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.connectionFactory = new SqlConnectionFactory(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GroundsIce.DB.Test;Integrated Security=True;Pooling=False");
        }

        [SetUp]
        public async Task SetUp()
        {
            this.accountRepository = new DbAccountRepository_OLD(this.connectionFactory);
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
            Account_OLD account = await this.accountRepository.CreateAccountAsync(ValidLogin, ValidPassword);
            Profile profile = await this.subject.GetProfileAsync(account.UserId + 1);
            Assert.IsNull(profile);
        }

        [Test]
        public async Task GetProfileAsync_ReturnNotNullProfile_When_PassingExistingUserId()
        {
            Account_OLD account = await this.accountRepository.CreateAccountAsync(ValidLogin, ValidPassword);
            Profile profile = await this.subject.GetProfileAsync(account.UserId);
            Assert.IsNotNull(profile);
            Assert.AreEqual(profile.Login, ValidLogin);
        }

        [Test]
        public void SetProfileInfoAsync_ThrowArgumentNullException_When_PassingNullProfileInfo()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.subject.SetProfileInfoAsync(1, null));
        }

        [Test]
        public void SetProfileInfoAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.subject.SetProfileInfoAsync(-1, new List<ProfileInfoEntry>()));
        }

        [Test]
        public void SetProfileInfoAsync_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZeroAndNullProfileInfo()
        {
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.subject.SetProfileInfoAsync(-1, null));
        }

        [Test]
        public async Task SetProfileInfoAsync_ReturnFalse_When_PassingUserIdNotExistsWithEmptyProfileInfo()
        {
            bool updated = await this.subject.SetProfileInfoAsync(1, new List<ProfileInfoEntry>());
            Assert.IsFalse(updated);
        }

        [Test]
        public async Task SetProfileInfoAsync_ReturnFalse_When_PassingUserIdNotExistsWithNotEmptyProfileInfo()
        {
            var profileInfo = new List<ProfileInfoEntry>
            {
                new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = true }
            };
            bool updated = await this.subject.SetProfileInfoAsync(1, profileInfo);
            Assert.IsFalse(updated);
        }

        [Test]
        public async Task SetProfileInfoAsync_ReturnTrue_When_PassingEmptyProfileInfo()
        {
            Account_OLD account = await this.accountRepository.CreateAccountAsync(ValidLogin, ValidPassword);
            bool updated = await this.subject.SetProfileInfoAsync(account.UserId, new List<ProfileInfoEntry>());
            Assert.IsTrue(updated);
        }

        [Test]
        public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingEmptyProfileInfo()
        {
            Account_OLD account = await this.accountRepository.CreateAccountAsync(ValidLogin, ValidPassword);
            await this.subject.SetProfileInfoAsync(account.UserId, new List<ProfileInfoEntry>());
            Profile profile = await this.subject.GetProfileAsync(account.UserId);
            Assert.AreEqual(profile.ProfileInfo.Count(), 0);
        }

        public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(long userId, List<ProfileInfoEntry> profileInfo)
        {
            await this.subject.SetProfileInfoAsync(userId, profileInfo);
            Profile profile = await this.subject.GetProfileAsync(userId);
            Assert.AreEqual(profile.Login, ValidLogin);
            Assert.AreEqual(profile.ProfileInfo.Count(), profileInfo.Count());
            foreach (var entry in profile.ProfileInfo)
            {
                Assert.AreEqual(profileInfo.FindAll(v => v.Equals(entry)).Count(), 1);
            }
        }

        [Test]
        public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo()
        {
            Account_OLD account = await this.accountRepository.CreateAccountAsync(ValidLogin, ValidPassword);
            foreach (var typeName in Enum.GetNames(typeof(ProfileInfoType)))
            {
                var type = (ProfileInfoType)Enum.Parse(typeof(ProfileInfoType), typeName);
                await this.SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
                    account.UserId,
                    new[]
                    {
                        new ProfileInfoEntry() { Type = type, Value = "a", IsPublic = true }
                    }.ToList());
                await this.SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
                    account.UserId,
                    new[]
                    {
                        new ProfileInfoEntry() { Type = type, Value = "b", IsPublic = false }
                    }.ToList());
            }
        }

        [Test]
        public async Task SetProfileInfoAsync_AffectGetProfileAsync_When_PassingFullProfileInfo()
        {
            Account_OLD account = await this.accountRepository.CreateAccountAsync(ValidLogin, ValidPassword);
            var profileInfo = new List<ProfileInfoEntry>
            {
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = false },
                new ProfileInfoEntry { Type = ProfileInfoType.LastName, Value = "b", IsPublic = true },
                new ProfileInfoEntry { Type = ProfileInfoType.MiddleName, Value = "c", IsPublic = false },
                new ProfileInfoEntry { Type = ProfileInfoType.Description, Value = "d", IsPublic = true },
                new ProfileInfoEntry { Type = ProfileInfoType.City, Value = "e", IsPublic = false },
                new ProfileInfoEntry { Type = ProfileInfoType.Region, Value = "f", IsPublic = false },
            };
            await this.SetProfileInfoAsync_AffectGetProfileAsync_When_PassingProfileInfo(
                account.UserId,
                profileInfo);
        }

        [Test]
        public async Task SetProfileInfoAsync_NotAffectGetProfileAsync_When_PassingProfileInfoWithNullOrEmptyValue()
        {
            Account_OLD account = await this.accountRepository.CreateAccountAsync(ValidLogin, ValidPassword);
            await this.subject.SetProfileInfoAsync(account.UserId, new List<ProfileInfoEntry> { new ProfileInfoEntry { Value = null } });
            await this.subject.SetProfileInfoAsync(account.UserId, new List<ProfileInfoEntry> { new ProfileInfoEntry { Value = string.Empty } });
            Profile profile = await this.subject.GetProfileAsync(account.UserId);
            Assert.AreEqual(profile.ProfileInfo.Count(), 0);
        }

        [Test]
        public async Task SetProfileInfoAsync_NotAffectGetProfileAsync_For_OtherUsers()
        {
            Account_OLD account1 = await this.accountRepository.CreateAccountAsync(ValidLogin, ValidPassword);
            Account_OLD account2 = await this.accountRepository.CreateAccountAsync(AnotherValidLogin, AnotherValidPassword);
            var profileInfo1 = new List<ProfileInfoEntry>
            {
                new ProfileInfoEntry { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = false },
                new ProfileInfoEntry { Type = ProfileInfoType.LastName, Value = "b", IsPublic = true },
                new ProfileInfoEntry { Type = ProfileInfoType.MiddleName, Value = "c", IsPublic = false },
            };
            var profileInfo2 = new List<ProfileInfoEntry>
            {
                new ProfileInfoEntry { Type = ProfileInfoType.MiddleName, Value = "c", IsPublic = false },
                new ProfileInfoEntry { Type = ProfileInfoType.City, Value = "d", IsPublic = true },
                new ProfileInfoEntry { Type = ProfileInfoType.Description, Value = "e", IsPublic = false },
            };
            await this.subject.SetProfileInfoAsync(account1.UserId, profileInfo1);
            await this.subject.SetProfileInfoAsync(account2.UserId, profileInfo2);
            Profile profile1 = await this.subject.GetProfileAsync(account1.UserId);
            Assert.AreEqual(profile1.Login, ValidLogin);
            Assert.AreEqual(profile1.ProfileInfo.Count(), profileInfo1.Count());
            foreach (var entry in profile1.ProfileInfo)
            {
                Assert.AreEqual(profileInfo1.FindAll(v => v.Equals(entry)).Count(), 1);
            }

            Profile profile2 = await this.subject.GetProfileAsync(account2.UserId);
            Assert.AreEqual(profile2.Login, AnotherValidLogin);
            Assert.AreEqual(profile2.ProfileInfo.Count(), profileInfo1.Count());
            foreach (var entry in profile2.ProfileInfo)
            {
                Assert.AreEqual(profileInfo2.FindAll(v => v.Equals(entry)).Count(), 1);
            }

            await this.subject.SetProfileInfoAsync(account2.UserId, new List<ProfileInfoEntry>());
            profile1 = await this.subject.GetProfileAsync(account1.UserId);
            Assert.AreEqual(profile1.Login, ValidLogin);
            Assert.AreEqual(profile1.ProfileInfo.Count(), profileInfo1.Count());
            foreach (var entry in profile1.ProfileInfo)
            {
                Assert.AreEqual(profileInfo1.FindAll(v => v.Equals(entry)).Count(), 1);
            }

            profile2 = await this.subject.GetProfileAsync(account2.UserId);
            Assert.AreEqual(profile2.Login, AnotherValidLogin);
            Assert.AreEqual(profile2.ProfileInfo.Count(), 0);
        }
    }
}
