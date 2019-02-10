using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroundsIce.Model.Abstractions.Validators;
using GroundsIce.Model.Abstractions.Repositories;
using GroundsIce.Model.Entities;
using GroundsIce.WebApi.DTO.Common;
using System.Net.Http;

namespace GroundsIce.WebApi.Controllers.Profile.Tests
{
	using Profile = GroundsIce.Model.Entities.Profile;

	[TestFixture]
	class ProfileControllerTests
	{
		private Mock<IProfileInfoValidator> _mockProfileInfoValidator;
		private Mock<IProfileRepository> _mockProfileRepository;
		private List<ProfileInfoEntry> _validProfileInfo;
		private List<ProfileInfoEntry> _invalidProfileInfo;
		private long _validUserId;
		private long _invalidUserId;
		private Profile _validProfile;
		private ProfileController _subject;
		private long _ownUserId;

		[SetUp]
		public void SetUp()
		{
			_ownUserId = 1;
			_validUserId = 2;
			_invalidUserId = 3;

			_validProfileInfo = new[] { new ProfileInfoEntry() }.ToList();
			_invalidProfileInfo = new[] { new ProfileInfoEntry() }.ToList();

			_validProfile = new Profile();
			_validProfile.ProfileInfo = new[] { new ProfileInfoEntry() }.ToList();

			_mockProfileInfoValidator = new Mock<IProfileInfoValidator>();
			_mockProfileInfoValidator.Setup(c => c.ValidateAsync(It.Is<List<ProfileInfoEntry>>(v => v != _validProfileInfo))).ReturnsAsync(false);
			_mockProfileInfoValidator.Setup(c => c.ValidateAsync(_validProfileInfo)).ReturnsAsync(true);

			_mockProfileRepository = new Mock<IProfileRepository>();
			_mockProfileRepository.Setup(c => c.GetProfileAsync(_validUserId)).ReturnsAsync(_validProfile);
			_mockProfileRepository.Setup(c => c.GetProfileAsync(_invalidUserId)).ReturnsAsync((Profile)null);
			_mockProfileRepository.Setup(c => c.SetProfileInfoAsync(_validUserId, _validProfileInfo)).ReturnsAsync(true);
			_mockProfileRepository.Setup(c => c.SetProfileInfoAsync(_invalidUserId, _validProfileInfo)).ReturnsAsync(false);

			_subject = new ProfileController(new[] { _mockProfileInfoValidator.Object }, _mockProfileRepository.Object);
		}

		[Test]
		public void Ctor_ThrowArgumentNullException_When_PassingNullArgs()
		{
			Assert.Throws<ArgumentNullException>(() => new ProfileController(null, null));
			Assert.Throws<ArgumentNullException>(() => new ProfileController(null, _mockProfileRepository.Object));
			Assert.Throws<ArgumentNullException>(() => new ProfileController(new[] { _mockProfileInfoValidator.Object }, null));
		}

		[Test]
		public void Ctor_DoesNotThrow_When_PassingNotNull()
		{
			Assert.DoesNotThrow(() => new ProfileController(new[] { _mockProfileInfoValidator.Object }, _mockProfileRepository.Object));
		}

		[Test]
		public void Get_ThrowArgumentNullException_When_PassingNullArg()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _subject.Get(null));
		}

		[Test]
		public async Task Get_ReturnSuccessWithProfile_When_PassingValidUserId()
		{
			Value<Profile> result = await _subject.Get(new DTO.ProfileRequest { UserId = _validUserId.ToString() });
			Assert.AreEqual(result.Type, (int)ProfileController.ValueType.Success);
			Assert.AreEqual(result.Payload, _validProfile);
			_mockProfileRepository.Verify(c => c.GetProfileAsync(_validUserId));
		}

		[Test]
		public async Task Get_ReturnProfileNotExists_When_PassingInvalidUserId()
		{
			Value<Profile> result = await _subject.Get(new DTO.ProfileRequest { UserId = _invalidUserId.ToString() });
			Assert.AreEqual(result.Type, (int)ProfileController.ValueType.ProfileNotExists);
			_mockProfileRepository.Verify(c => c.GetProfileAsync(_invalidUserId));
		}

		[Test]
		public void Get_ThrowArgumentOutOfRangeException_When_PassingUserIdIsLessThenZero()
		{
			long negativeUserId = -1;
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _subject.Get(new DTO.ProfileRequest { UserId = negativeUserId.ToString() }));
		}

		private void SetUserIdToRequest(long userId)
		{
			_subject.Request = new HttpRequestMessage();
			_subject.Request.Properties["USER_ID"] = userId;
		}

		[Test]
		public async Task Get_CutDownNotPublicProfileEntriesFromResult_When_PassingNotOwnUserId()
		{
			SetUserIdToRequest(_ownUserId);
			var publicProfileInfo = new ProfileInfoEntry() { Type = ProfileInfoType.MiddleName, Value = "a", IsPublic = true };
			_validProfile.ProfileInfo = new[] {
				new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = false },
				publicProfileInfo
			}.ToList();
			Value<Profile> result = await _subject.Get(new DTO.ProfileRequest { UserId = _validUserId.ToString() });
			Assert.AreEqual(result.Payload.ProfileInfo.Count, 1);
			Assert.AreEqual(result.Payload.ProfileInfo.First(), publicProfileInfo);
		}

		[Test]
		public async Task Get_CutDownNotPublicProfileEntriesFromResult_When_AnonymousCall()
		{
			var publicProfileInfo = new ProfileInfoEntry() { Type = ProfileInfoType.MiddleName, Value = "a", IsPublic = true };
			_validProfile.ProfileInfo = new[] {
				new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = false },
				publicProfileInfo
			}.ToList();
			Value<Profile> result = await _subject.Get(new DTO.ProfileRequest { UserId = _validUserId.ToString() });
			Assert.AreEqual(result.Payload.ProfileInfo.Count, 1);
			Assert.AreEqual(result.Payload.ProfileInfo.First(), publicProfileInfo);
		}

		[Test]
		public async Task Get_DoesNotCutDownNotPublicProfileEntriesFromResult_When_PassingOwnUserId()
		{
			SetUserIdToRequest(_validUserId);
			var privateProfileInfo = new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = false };
			var publicProfileInfo = new ProfileInfoEntry() { Type = ProfileInfoType.MiddleName, Value = "b", IsPublic = true };
			_validProfile.ProfileInfo = new[] {
				privateProfileInfo,
				publicProfileInfo
			}.ToList();
			Value<Profile> result = await _subject.Get(new DTO.ProfileRequest { UserId = _validUserId.ToString() });
			Assert.AreEqual(result.Payload.ProfileInfo.Count, 2);
			Assert.AreEqual(result.Payload.ProfileInfo.First(), privateProfileInfo);
			Assert.AreEqual(result.Payload.ProfileInfo.Last(), publicProfileInfo);
		}

		[Test]
		public void SetProfileInfo_ThrowArgumentNullException_When_PassingNullArg()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _subject.SetProfileInfo(null));
		}

		[Test]
		public void SetProfileInfo_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
		{
			var dto = new DTO.ProfileInfoModel { ProfileInfo = _validProfileInfo };
			Assert.ThrowsAsync<ArgumentNullException>(() => _subject.SetProfileInfo(dto));
		}

		[Test]
		public async Task SetProfileInfo_ReturnBadData_When_PassingInvalidProfileInfoThroughValidators()
		{
			SetUserIdToRequest(_validUserId);
			var dto = new DTO.ProfileInfoModel { ProfileInfo = _invalidProfileInfo };
			Value result = await _subject.SetProfileInfo(dto);
			Assert.AreEqual(result.Type, (int)ProfileController.ValueType.BadData);
			_mockProfileRepository.VerifyNoOtherCalls();
		}

		[Test]
		public async Task SetProfileInfo_ReturnSuccess_When_PassingValidProfileInfoThroughValidatorsAndUpdated()
		{
			SetUserIdToRequest(_validUserId);
			var dto = new DTO.ProfileInfoModel { ProfileInfo = _validProfileInfo };
			Value result = await _subject.SetProfileInfo(dto);
			Assert.AreEqual(result.Type, (int)ProfileController.ValueType.Success);
			_mockProfileRepository.Verify(c => c.SetProfileInfoAsync(_validUserId, _validProfileInfo));
		}

		[Test]
		public async Task SetProfileInfo_ReturnBadData_When_PassingValidProfileInfoButInvalidUserId()
		{
			SetUserIdToRequest(_invalidUserId);
			var dto = new DTO.ProfileInfoModel { ProfileInfo = _validProfileInfo };
			Value result = await _subject.SetProfileInfo(dto);
			Assert.AreEqual(result.Type, (int)ProfileController.ValueType.BadData);
			_mockProfileRepository.Verify(c => c.SetProfileInfoAsync(_invalidUserId, _validProfileInfo));
		}
	}
}
