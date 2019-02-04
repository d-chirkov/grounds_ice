using GroundsIce.Model.Abstractions.Validators;
using GroundsIce.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model.Validators.ProfileInfo
{
	public class UniqueTypesValidator : IProfileInfoValidator
	{
		public Task<bool> ValidateAsync(List<ProfileInfoEntry> profileInfo)
		{
			if (profileInfo == null) throw new ArgumentNullException("profileInfo");
			return Task.FromResult(profileInfo.Count == profileInfo.GroupBy(v => v.Type).Select(group => group.First()).Count());
		}
	}
}
