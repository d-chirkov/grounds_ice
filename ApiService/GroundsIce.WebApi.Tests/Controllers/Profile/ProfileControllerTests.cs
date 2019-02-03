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
		private ProfileInfo _validProfileInfo;
		private ProfileInfo _invalidProfileInfo;
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

			_validProfileInfo = new ProfileInfo();
			_invalidProfileInfo = new ProfileInfo();

			_validProfile = new Profile();
			_validProfile.ProfileInfo = new ProfileInfo();

			_mockProfileInfoValidator = new Mock<IProfileInfoValidator>();
			_mockProfileInfoValidator.Setup(c => c.ValidateAsync(It.Is<ProfileInfo>(v => v != _validProfileInfo))).ReturnsAsync(false);
			_mockProfileInfoValidator.Setup(c => c.ValidateAsync(_validProfileInfo)).ReturnsAsync(true);

			_mockProfileRepository = new Mock<IProfileRepository>();
			_mockProfileRepository.Setup(c => c.GetProfileAsync(_validUserId)).ReturnsAsync(_validProfile);
			_mockProfileRepository.Setup(c => c.GetProfileAsync(_invalidUserId)).ReturnsAsync((Profile)null);
			_mockProfileRepository.Setup(c => c.SetProfileInfoAsync(_validUserId, _validProfileInfo)).ReturnsAsync(true);
			_mockProfileRepository.Setup(c => c.SetProfileInfoAsync(_invalidUserId, _validProfileInfo)).ReturnsAsync(false);

			_subject = new ProfileController(_mockProfileInfoValidator.Object, _mockProfileRepository.Object);
		}

		[Test]
		public void Ctor_ThrowArgumentNullException_When_PassingNullArgs()
		{
			Assert.Throws<ArgumentNullException>(() => new ProfileController(null, null));
			Assert.Throws<ArgumentNullException>(() => new ProfileController(null, _mockProfileRepository.Object));
			Assert.Throws<ArgumentNullException>(() => new ProfileController(_mockProfileInfoValidator.Object, null));
		}

		[Test]
		public void Ctor_DoesNotThrow_When_PassingNotNull()
		{
			Assert.DoesNotThrow(() => new ProfileController(_mockProfileInfoValidator.Object, _mockProfileRepository.Object));
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
			_validProfile.ProfileInfo = new ProfileInfo();
			_validProfile.ProfileInfo.FirstName = new ProfileInfoEntry() { Value = "a", IsPublic = false };
			_validProfile.ProfileInfo.MiddleName = new ProfileInfoEntry() { Value = "b", IsPublic = true };
			Value<Profile> result = await _subject.Get(new DTO.ProfileRequest { UserId = _validUserId.ToString() });
			Assert.IsNull(result.Payload.ProfileInfo.FirstName);
			Assert.NotNull(result.Payload.ProfileInfo.MiddleName);
		}

		[Test]
		public async Task Get_DoesNotCutDownNotPublicProfileEntriesFromResult_When_PassingNotOwnUserId()
		{
			_subject.Request = new HttpRequestMessage();
			_subject.Request.Properties["USER_ID"] = _validUserId;
			_validProfile.ProfileInfo = new ProfileInfo();
			_validProfile.ProfileInfo.FirstName = new ProfileInfoEntry() { Value = "a", IsPublic = true };
			_validProfile.ProfileInfo.MiddleName = new ProfileInfoEntry() { Value = "b", IsPublic = false };
			Value<Profile> result = await _subject.Get(new DTO.ProfileRequest { UserId = _validUserId.ToString() });
			Assert.NotNull(result.Payload.ProfileInfo.FirstName);
			Assert.NotNull(result.Payload.ProfileInfo.MiddleName);
		}

		[Test]
		public void SetProfileInfo_ThrowArgumentNullException_When_PassingNullArg()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _subject.SetProfileInfo(null));
		}

		[Test]
		public void SetProfileInfo_ThrowArgumentNullException_When_UserIdNotSpecifiedInRequest()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _subject.SetProfileInfo(_validProfileInfo));
		}

		[Test]
		public async Task SetProfileInfo_ReturnBadData_When_PassingInvalidProfileInfoThroughValidators()
		{
			SetUserIdToRequest(_validUserId);
			Value result = await _subject.SetProfileInfo(_invalidProfileInfo);
			Assert.AreEqual(result.Type, (int)ProfileController.ValueType.BadData);
			_mockProfileRepository.VerifyNoOtherCalls();
		}

		[Test]
		public async Task SetProfileInfo_ReturnSuccess_When_PassingValidProfileInfoThroughValidatorsAndUpdated()
		{
			SetUserIdToRequest(_validUserId);
			Value result = await _subject.SetProfileInfo(_validProfileInfo);
			Assert.AreEqual(result.Type, (int)ProfileController.ValueType.Success);
			_mockProfileRepository.Verify(c => c.SetProfileInfoAsync(_validUserId, _validProfileInfo));
		}

		[Test]
		public async Task SetProfileInfo_ReturnBadData_When_PassingValidProfileInfoButInvalidUserId()
		{
			SetUserIdToRequest(_invalidUserId);
			Value result = await _subject.SetProfileInfo(_validProfileInfo);
			Assert.AreEqual(result.Type, (int)ProfileController.ValueType.BadData);
			_mockProfileRepository.Verify(c => c.SetProfileInfoAsync(_invalidUserId, _validProfileInfo));
		}
	}
}
