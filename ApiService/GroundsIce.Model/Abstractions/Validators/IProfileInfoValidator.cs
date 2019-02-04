using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroundsIce.Model.Entities;

namespace GroundsIce.Model.Abstractions.Validators
{
	public interface IProfileInfoValidator
	{
		Task<bool> ValidateAsync(List<ProfileInfoEntry> profileInfo);
	}
}
