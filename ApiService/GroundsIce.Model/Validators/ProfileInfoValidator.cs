using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroundsIce.Model.Abstractions.Validators;
using GroundsIce.Model.Entities;

namespace GroundsIce.Model.Validators
{
	public class ProfileInfoValidator : IProfileInfoValidator
	{
		public ProfileInfoValidator()
		{
			_firstNameValidators = new List<IStringValidator>();
			_middelNameValidators = new List<IStringValidator>();
			_surnameValidators = new List<IStringValidator>();
			_locationValidators = new List<IStringValidator>();
			_descriptionValidators = new List<IStringValidator>();
		}

		public async Task<bool> ValidateAsync(ProfileInfo profileInfo)
		{
			if (profileInfo == null) throw new ArgumentNullException("profileInfo");
			return
				await ValidateWithAsync(_firstNameValidators, profileInfo.FirstName) &&
				await ValidateWithAsync(_middelNameValidators, profileInfo.MiddleName) &&
				await ValidateWithAsync(_surnameValidators, profileInfo.Surname) &&
				await ValidateWithAsync(_locationValidators, profileInfo.Location) &&
				await ValidateWithAsync(_descriptionValidators, profileInfo.Description);
		}

		private async Task<bool> ValidateWithAsync(ICollection<IStringValidator> validators, ProfileInfoEntry entry)
		{
			if (entry != null)
			{
				foreach (var validator in validators)
				{
					if (!await validator.ValidateAsync(entry.Value))
					{
						return false;
					}
				}
			}
			return true;
		}

		private ICollection<IStringValidator> _firstNameValidators;

		public void AddFirstNameValidator(IStringValidator validator)
		{
			_firstNameValidators.Add(validator ?? throw new ArgumentNullException("validator"));
		}

		private ICollection<IStringValidator> _middelNameValidators;

		public void AddMiddelNameValidator(IStringValidator validator)
		{
			_middelNameValidators.Add(validator ?? throw new ArgumentNullException("validator"));
		}

		private ICollection<IStringValidator> _surnameValidators;

		public void AddSurameValidator(IStringValidator validator)
		{
			_surnameValidators.Add(validator ?? throw new ArgumentNullException("validator"));
		}

		private ICollection<IStringValidator> _locationValidators;

		public void AddLocationValidator(IStringValidator validator)
		{
			_locationValidators.Add(validator ?? throw new ArgumentNullException("validator"));
		}

		private ICollection<IStringValidator> _descriptionValidators;

		public void AddDescriptionValidator(IStringValidator validator)
		{
			_descriptionValidators.Add(validator ?? throw new ArgumentNullException("validator"));
		}
	}
}
