using GroundsIce.WebApi.Attributes;
using GroundsIce.WebApi.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using GroundsIce.Model.Entities;
using GroundsIce.Model.Abstractions.Validators;
using GroundsIce.Model.Abstractions.Repositories;

namespace GroundsIce.WebApi.Controllers.Profile
{
	using Profile = GroundsIce.Model.Entities.Profile ;

	[AuthorizeApi]
	[RoutePrefix("api/profile")]
	public class ProfileController : ApiController
	{
		public enum ValueType
		{
			Success = 1000,
			ProfileNotExists = 2000,
			BadData = 3000,
		}

		private IEnumerable<IProfileInfoValidator> _profileInfoValidators;
		private IProfileRepository _profileRepository;

		public ProfileController(IEnumerable<IProfileInfoValidator> profileInfoValidators, IProfileRepository profileRepository)
		{
			_profileInfoValidators = profileInfoValidators ?? throw new ArgumentNullException("profileInfoValidator");
			_profileRepository = profileRepository ?? throw new ArgumentNullException("profileRepository");
		}

		[Route("get")]
		[AllowAnonymous]
		[HttpPost]
		public async Task<Value<Profile>> Get(DTO.ProfileRequest dto)
		{
			if (dto == null) throw new ArgumentNullException("dto");
			long requestedUserId = long.Parse(dto.UserId ?? throw new ArgumentNullException("dto.UserId"));
			if (requestedUserId < 0) throw new ArgumentOutOfRangeException("requestedUserId");
			Profile profile = await _profileRepository.GetProfileAsync(requestedUserId);
			if (profile == null)
			{
				return new Value<Profile>((int)ValueType.ProfileNotExists);
			}
			long? ownUserId = GetUserIdFromRequest();
			if (!ownUserId.HasValue || (ownUserId.HasValue && ownUserId.Value != requestedUserId))
			{
				profile.ProfileInfo = profile.ProfileInfo.Where(v => v.IsPublic).ToList();
			}
			return new Value<Profile>((int)ValueType.Success) { Payload = profile };
		}

		private bool NeedToCutdownEntry(ProfileInfoEntry entry)
		{
			return (entry != null && !entry.IsPublic);
		}

		[Route("set_profile_info")]
		[HttpPost]
		public async Task<Value> SetProfileInfo(DTO.ProfileInfoModel dto)
		{
			if (dto == null) throw new ArgumentNullException("dto");
			if (dto.ProfileInfo == null) throw new ArgumentNullException("dto.ProfileInfo");
			long userId = GetUserIdFromRequest() ?? throw new ArgumentNullException("userId");
			foreach (var validator in _profileInfoValidators)
			{
				if (!await validator.ValidateAsync(dto.ProfileInfo))
				{
					return new Value((int)ValueType.BadData);
				}
			}
			bool updated = await _profileRepository.SetProfileInfoAsync(userId, dto.ProfileInfo);
			return updated ? new Value((int)ValueType.Success) : new Value((int)ValueType.BadData);
		}

		private long? GetUserIdFromRequest()
		{
			return Request != null && Request.Properties.ContainsKey("USER_ID") ? (long?)(Request?.Properties["USER_ID"]) : null;
		}
	}
}