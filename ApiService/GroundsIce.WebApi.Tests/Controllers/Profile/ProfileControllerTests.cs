namespace GroundsIce.WebApi.Controllers.Profile.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using GroundsIce.Model.Abstractions.Repositories;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.Model.Entities;
    using GroundsIce.WebApi.DTO.Common;
    using Moq;
    using NUnit.Framework;
    using Profile_OLD = Model.Entities.Profile_OLD;

    [TestFixture]
    public class ProfileControllerTests
    {
        private Mock<IProfileInfoValidator_OLD> mockProfileInfoValidator;
        private Mock<IProfileRepository_OLD> mockProfileRepository;
        private List<ProfileEntry> validProfileInfo;
        private List<ProfileEntry> invalidProfileInfo;
        private long validUserId;
        private long invalidUserId;
        private Profile_OLD validProfile;
        private ProfileController subject;
        private long ownUserId;

        [SetUp]
        public void SetUp()
        {
            this.ownUserId = 1;
            this.validUserId = 2;
            this.invalidUserId = 3;

            this.validProfileInfo = new[] { new ProfileEntry() }.ToList();
            this.invalidProfileInfo = new[] { new ProfileEntry() }.ToList();

            this.validProfile = new Profile_OLD
            {
                ProfileInfo = new[] { new ProfileEntry() }.ToList()
            };

            this.mockProfileInfoValidator = new Mock<IProfileInfoValidator_OLD>();
            this.mockProfileInfoValidator.Setup(c => c.ValidateAsync(It.Is<List<ProfileEntry>>(v => v != this.validProfileInfo))).ReturnsAsync(false);
            this.mockProfileInfoValidator.Setup(c => c.ValidateAsync(this.validProfileInfo)).ReturnsAsync(true);

            this.mockProfileRepository = new Mock<IProfileRepository_OLD>();
            this.mockProfileRepository.Setup(c => c.GetProfileAsync(this.validUserId)).ReturnsAsync(this.validProfile);
            this.mockProfileRepository.Setup(c => c.GetProfileAsync(this.invalidUserId)).ReturnsAsync((Profile_OLD)null);
            this.mockProfileRepository.Setup(c => c.SetProfileInfoAsync(this.validUserId, this.validProfileInfo)).ReturnsAsync(true);
            this.mockProfileRepository.Setup(c => c.SetProfileInfoAsync(this.invalidUserId, this.validProfileInfo)).ReturnsAsync(false);

            this.subject = new ProfileController(this.mockProfileInfoValidator.Object, this.mockProfileRepository.Object);
        }

        [Test]
        public void Ctor_ThrowArgumentNullException_When_PassingNullArgs()
        {
            Assert.Throws<ArgumentNullException>(() => new ProfileController(null, null));
            Assert.Throws<ArgumentNullException>(() => new ProfileController(null, this.mockProfileRepository.Object));
            Assert.Throws<ArgumentNullException>(() => new ProfileController(this.mockProfileInfoValidator.Object, null));
        }

        [Test]
        public void Ctor_DoesNotThrow_When_PassingNotNull()
        {
            Assert.DoesNotThrow(() => new ProfileController(this.mockProfileInfoValidator.Object, this.mockProfileRepository.Object));
        }

        [Test]
        public void Get_ThrowArgumentNullException_When_PassingNullArg()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.subject.Get(null));
        }

        [Test]
        public async Task Get_ReturnSuccessWithProfile_When_PassingValidUserId()
        {
            Value<Profile_OLD> result = await this.subject.Get(new DTO.ProfileRequest { UserId = this.validUserId.ToString() });
            Assert.AreEqual(result.Type, (int)ProfileController.ValueType.Success);
            Assert.AreEqual(result.Payload, this.validProfile);
            this.mockProfileRepository.Verify(c => c.GetProfileAsync(this.validUserId));
        }

        [Test]
        public async Task Get_ReturnProfileNotExists_When_PassingInvalidUserId()
        {
            Value<Profile_OLD> result = await this.subject.Get(new DTO.ProfileRequest { UserId = this.invalidUserId.ToString() });
            Assert.AreEqual(result.Type, (int)ProfileController.ValueType.ProfileNotExists);
            this.mockProfileRepository.Verify(c => c.GetProfileAsync(this.invalidUserId));
        }

        [Test]
        public void Get_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
        {
            long negativeUserId = -1;
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.subject.Get(new DTO.ProfileRequest { UserId = negativeUserId.ToString() }));
        }

        public void SetUserIdToRequest(long userId)
        {
            this.subject.Request = new HttpRequestMessage();
            this.subject.Request.Properties["USER_ID"] = userId;
        }

        [Test]
        public async Task Get_CutDownNotPublicProfileEntriesFromResult_When_PassingNotOwnUserId()
        {
            this.SetUserIdToRequest(this.ownUserId);
            var publicProfileInfo = new ProfileEntry() { Type = ProfileEntryType.MiddleName, Value = "a", IsPublic = true };
            this.validProfile.ProfileInfo = new[]
            {
                new ProfileEntry() { Type = ProfileEntryType.FirstName, Value = "a", IsPublic = false },
                publicProfileInfo
            }.ToList();
            Value<Profile_OLD> result = await this.subject.Get(new DTO.ProfileRequest { UserId = this.validUserId.ToString() });
            Assert.AreEqual(result.Payload.ProfileInfo.Count, 1);
            Assert.AreEqual(result.Payload.ProfileInfo.First(), publicProfileInfo);
        }

        [Test]
        public async Task Get_CutDownNotPublicProfileEntriesFromResult_When_AnonymousCall()
        {
            var publicProfileInfo = new ProfileEntry() { Type = ProfileEntryType.MiddleName, Value = "a", IsPublic = true };
            this.validProfile.ProfileInfo = new[]
            {
                new ProfileEntry() { Type = ProfileEntryType.FirstName, Value = "a", IsPublic = false },
                publicProfileInfo
            }.ToList();
            Value<Profile_OLD> result = await this.subject.Get(new DTO.ProfileRequest { UserId = this.validUserId.ToString() });
            Assert.AreEqual(result.Payload.ProfileInfo.Count, 1);
            Assert.AreEqual(result.Payload.ProfileInfo.First(), publicProfileInfo);
        }

        [Test]
        public async Task Get_DoesNotCutDownNotPublicProfileEntriesFromResult_When_PassingOwnUserId()
        {
            this.SetUserIdToRequest(this.validUserId);
            var privateProfileInfo = new ProfileEntry() { Type = ProfileEntryType.FirstName, Value = "a", IsPublic = false };
            var publicProfileInfo = new ProfileEntry() { Type = ProfileEntryType.MiddleName, Value = "b", IsPublic = true };
            this.validProfile.ProfileInfo = new[]
            {
                privateProfileInfo,
                publicProfileInfo
            }.ToList();
            Value<Profile_OLD> result = await this.subject.Get(new DTO.ProfileRequest { UserId = this.validUserId.ToString() });
            Assert.AreEqual(result.Payload.ProfileInfo.Count, 2);
            Assert.AreEqual(result.Payload.ProfileInfo.First(), privateProfileInfo);
            Assert.AreEqual(result.Payload.ProfileInfo.Last(), publicProfileInfo);
        }

        [Test]
        public void SetProfileInfo_ThrowArgumentNullException_When_PassingNullArg()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => this.subject.SetProfileInfo(null));
        }

        [Test]
        public void SetProfileInfo_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
        {
            var dto = new DTO.ProfileInfoModel { ProfileInfo = this.validProfileInfo };
            Assert.ThrowsAsync<ArgumentNullException>(() => this.subject.SetProfileInfo(dto));
        }

        [Test]
        public async Task SetProfileInfo_ReturnBadData_When_PassingInvalidProfileInfoThroughValidators()
        {
            this.SetUserIdToRequest(this.validUserId);
            var dto = new DTO.ProfileInfoModel { ProfileInfo = this.invalidProfileInfo };
            Value result = await this.subject.SetProfileInfo(dto);
            Assert.AreEqual(result.Type, (int)ProfileController.ValueType.BadData);
            this.mockProfileRepository.VerifyNoOtherCalls();
        }

        [Test]
        public async Task SetProfileInfo_ReturnSuccess_When_PassingValidProfileInfoThroughValidatorsAndUpdated()
        {
            this.SetUserIdToRequest(this.validUserId);
            var dto = new DTO.ProfileInfoModel { ProfileInfo = this.validProfileInfo };
            Value result = await this.subject.SetProfileInfo(dto);
            Assert.AreEqual(result.Type, (int)ProfileController.ValueType.Success);
            this.mockProfileRepository.Verify(c => c.SetProfileInfoAsync(this.validUserId, this.validProfileInfo));
        }

        [Test]
        public async Task SetProfileInfo_ReturnBadData_When_PassingValidProfileInfoButInvalidUserId()
        {
            this.SetUserIdToRequest(this.invalidUserId);
            var dto = new DTO.ProfileInfoModel { ProfileInfo = this.validProfileInfo };
            Value result = await this.subject.SetProfileInfo(dto);
            Assert.AreEqual(result.Type, (int)ProfileController.ValueType.BadData);
            this.mockProfileRepository.Verify(c => c.SetProfileInfoAsync(this.invalidUserId, this.validProfileInfo));
        }
    }
}
