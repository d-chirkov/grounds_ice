using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using GroundsIce.Model.Abstractions.Validators;
using GroundsIce.Model.Entities;

namespace GroundsIce.Model.Validators.Tests
{
	[TestFixture]
	class ProfileInfoValidatorTests
	{
		private ProfileInfoValidator _subject;
		private Mock<IStringValidator> _mockValidator;
		private string _validValue;
		private string _invalidValue;
		private ProfileInfo _validProfileInfo;
		private ProfileInfo _invalidProfileInfo;

		[SetUp]
		public void SetUp()
		{
			_validValue = "a";
			_invalidValue = "b";
			_subject = new ProfileInfoValidator();
			_mockValidator = new Mock<IStringValidator>();
			_mockValidator.Setup(c => c.ValidateAsync(It.Is<string>(v => v != _validValue))).ReturnsAsync(false);
			_mockValidator.Setup(c => c.ValidateAsync(_validValue)).ReturnsAsync(true);
			_validProfileInfo = new ProfileInfo()
			{
				FirstName = new ProfileInfoEntry { Value = _validValue },
				MiddleName = new ProfileInfoEntry { Value = _validValue },
				Surname = new ProfileInfoEntry { Value = _validValue },
				Location = new ProfileInfoEntry { Value = _validValue },
				Description = new ProfileInfoEntry { Value = _validValue },
			};
			_invalidProfileInfo = new ProfileInfo()
			{
				FirstName = new ProfileInfoEntry { Value = _invalidValue },
				MiddleName = new ProfileInfoEntry { Value = _invalidValue },
				Surname = new ProfileInfoEntry { Value = _invalidValue },
				Location = new ProfileInfoEntry { Value = _invalidValue },
				Description = new ProfileInfoEntry { Value = _invalidValue },
			};
		}
		[Test]
		public void Ctor_DoesNotThrow_When_Create()
		{
			Assert.DoesNotThrow(() => new ProfileInfoValidator());
		}

		[Test]
		public void AddFirstNameValidator_ThrowArgumentNullException_When_PassingNullArgument()
		{
			Assert.Throws<ArgumentNullException>(() => _subject.AddFirstNameValidator(null));
		}

		[Test]
		public void AddMiddelNameValidator_ThrowArgumentNullException_When_PassingNullArgument()
		{
			Assert.Throws<ArgumentNullException>(() => _subject.AddMiddelNameValidator(null));
		}

		[Test]
		public void AddSurameValidator_ThrowArgumentNullException_When_PassingNullArgument()
		{
			Assert.Throws<ArgumentNullException>(() => _subject.AddSurameValidator(null));
		}

		[Test]
		public void AddLocationValidator_ThrowArgumentNullException_When_PassingNullArgument()
		{
			Assert.Throws<ArgumentNullException>(() => _subject.AddLocationValidator(null));
		}

		[Test]
		public void AddDescriptionValidator_ThrowArgumentNullException_When_PassingNullArgument()
		{
			Assert.Throws<ArgumentNullException>(() => _subject.AddDescriptionValidator(null));
		}

		[Test]
		public void AddFirstNameValidator_DoesNotThrow_When_PassingValidator()
		{
			Assert.DoesNotThrow(() => _subject.AddFirstNameValidator(_mockValidator.Object));
		}

		[Test]
		public void AddMiddelNameValidator_DoesNotThrow_When_PassingValidator()
		{
			Assert.DoesNotThrow(() => _subject.AddMiddelNameValidator(_mockValidator.Object));
		}

		[Test]
		public void AddSurameValidator_DoesNotThrow_When_PassingValidator()
		{
			Assert.DoesNotThrow(() => _subject.AddSurameValidator(_mockValidator.Object));
		}

		[Test]
		public void AddLocationValidator_DoesNotThrow_When_PassingValidator()
		{
			Assert.DoesNotThrow(() => _subject.AddLocationValidator(_mockValidator.Object));
		}

		[Test]
		public void AddDescriptionValidator_DoesNotThrow_When_PassingValidator()
		{
			Assert.DoesNotThrow(() => _subject.AddDescriptionValidator(_mockValidator.Object));
		}
		
		[Test]
		public async Task ValidateAsync_ReturnTrue_When_PassingValidProfileInfoThroughFirstNameValidator()
		{
			_subject.AddFirstNameValidator(_mockValidator.Object);
			Assert.IsTrue(await _subject.ValidateAsync(_validProfileInfo));
		}

		[Test]
		public async Task ValidateAsync_ReturnFalse_When_PassingInvalidProfileInfoThroughFirstNameValidator()
		{
			_subject.AddFirstNameValidator(_mockValidator.Object);
			Assert.IsFalse(await _subject.ValidateAsync(_invalidProfileInfo));
		}

		[Test]
		public async Task ValidateAsync_ReturnTrue_When_PassingValidProfileInfoThroughMiddleNameValidator()
		{
			_subject.AddMiddelNameValidator(_mockValidator.Object);
			Assert.IsTrue(await _subject.ValidateAsync(_validProfileInfo));
		}

		[Test]
		public async Task ValidateAsync_ReturnFalse_When_PassingInvalidProfileInfoThroughMiddleNameValidator()
		{
			_subject.AddMiddelNameValidator(_mockValidator.Object);
			Assert.IsFalse(await _subject.ValidateAsync(_invalidProfileInfo));
		}

		[Test]
		public async Task ValidateAsync_ReturnTrue_When_PassingValidProfileInfoThroughSurnameValidator()
		{
			_subject.AddSurameValidator(_mockValidator.Object);
			Assert.IsTrue(await _subject.ValidateAsync(_validProfileInfo));
		}

		[Test]
		public async Task ValidateAsync_ReturnFalse_When_PassingInvalidProfileInfoThroughSurnameValidator()
		{
			_subject.AddSurameValidator(_mockValidator.Object);
			Assert.IsFalse(await _subject.ValidateAsync(_invalidProfileInfo));
		}

		[Test]
		public async Task ValidateAsync_ReturnTrue_When_PassingValidProfileInfoThroughLocationValidator()
		{
			_subject.AddLocationValidator(_mockValidator.Object);
			Assert.IsTrue(await _subject.ValidateAsync(_validProfileInfo));
		}

		[Test]
		public async Task ValidateAsync_ReturnFalse_When_PassingInvalidProfileInfoThroughLocationValidator()
		{
			_subject.AddLocationValidator(_mockValidator.Object);
			Assert.IsFalse(await _subject.ValidateAsync(_invalidProfileInfo));
		}

		[Test]
		public async Task ValidateAsync_ReturnTrue_When_PassingValidProfileInfoThroughDescriptionValidator()
		{
			_subject.AddDescriptionValidator(_mockValidator.Object);
			Assert.IsTrue(await _subject.ValidateAsync(_validProfileInfo));
		}

		[Test]
		public async Task ValidateAsync_ReturnFalse_When_PassingInvalidProfileInfoThroughDescriptionValidator()
		{
			_subject.AddDescriptionValidator(_mockValidator.Object);
			Assert.IsFalse(await _subject.ValidateAsync(_invalidProfileInfo));
		}
	}
}
