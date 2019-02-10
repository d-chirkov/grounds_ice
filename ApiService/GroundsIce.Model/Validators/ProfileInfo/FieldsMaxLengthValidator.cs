using GroundsIce.Model.Abstractions.Validators;
using GroundsIce.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroundsIce.Model.Validators.ProfileInfo
{
	public class FieldsMaxLengthValidator : IProfileInfoValidator
	{
		public IDictionary<ProfileInfoType, int> TypesMaxLengths { get; set; }
		
		public Task<bool> ValidateAsync(List<ProfileInfoEntry> profileInfo)
		{
			if (profileInfo == null) throw new ArgumentNullException();
			bool validated = 
				TypesMaxLengths == null || 
				profileInfo.All(e => !TypesMaxLengths.ContainsKey(e.Type) || TypesMaxLengths[e.Type] >= e.Value.Length);
			return Task.FromResult(validated);
		}
	}
}
