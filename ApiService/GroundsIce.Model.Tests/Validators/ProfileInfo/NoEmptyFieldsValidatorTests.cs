using GroundsIce.Model.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model.Validators.ProfileInfo.Tests
{
	[TestFixture]
	class NoEmptyFieldsValidatorTests
	{
		private NoEmptyFieldsValidator _subject;

		[SetUp]
		public void SetUp()
		{
			_subject = new NoEmptyFieldsValidator();
		}

		[Test]
		public void ValidateAsync_ThrowArgumentNullException_When_PassingNullArg()
		{
			Assert.ThrowsAsync<ArgumentNullException>(() => _subject.ValidateAsync(null));
		}

		[Test]
		public async Task ValidateAsync_ReturnFalse_When_PassingOneEmptyField()
		{
			var arg = new[]
			{
				new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = "", IsPublic = true }
			}.ToList();
			Assert.IsFalse(await _subject.ValidateAsync(arg));
		}

		[Test]
		public async Task ValidateAsync_ReturnFalse_When_PassingTwoEmptyField()
		{
			var arg = new[]
			{
				new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = "", IsPublic = true },
				new ProfileInfoEntry() { Type = ProfileInfoType.LastName, Value = "", IsPublic = true }
			}.ToList();
			Assert.IsFalse(await _subject.ValidateAsync(arg));
		}

		[Test]
		public async Task ValidateAsync_ReturnFalse_When_OneOfTwoPassingFieldIsEmpty()
		{
			var arg = new[]
			{
				new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = "", IsPublic = true },
				new ProfileInfoEntry() { Type = ProfileInfoType.LastName, Value = "a", IsPublic = true }
			}.ToList();
			Assert.IsFalse(await _subject.ValidateAsync(arg));
		}

		[Test]
		public async Task ValidateAsync_ReturnTrue_When_PassingEmptyList()
		{
			Assert.IsTrue(await _subject.ValidateAsync(new List<ProfileInfoEntry>()));
		}

		[Test]
		public async Task ValidateAsync_ReturnTrue_When_PassingOneNotEmptyField()
		{
			var arg = new[]
			{
				new ProfileInfoEntry() { Type = ProfileInfoType.LastName, Value = "a", IsPublic = true }
			}.ToList();
			Assert.IsTrue(await _subject.ValidateAsync(arg));
		}

		[Test]
		public async Task ValidateAsync_ReturnTrue_When_PassingTwoNotEmptyField()
		{
			var arg = new[]
			{
				new ProfileInfoEntry() { Type = ProfileInfoType.FirstName, Value = "a", IsPublic = true },
				new ProfileInfoEntry() { Type = ProfileInfoType.LastName, Value = "v", IsPublic = true }
			}.ToList();
			Assert.IsTrue(await _subject.ValidateAsync(arg));
		}

	}
}
