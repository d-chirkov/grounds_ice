using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroundsIce.Model.Entities;

namespace GroundsIce.Model.Abstractions.Repositories
{
	public interface IProfileRepository
	{
		Task<Profile> GetProfileAsync(long userId);
		Task<bool> SetProfileInfoAsync(long userId, ProfileInfo profileInfo);
	}
}
