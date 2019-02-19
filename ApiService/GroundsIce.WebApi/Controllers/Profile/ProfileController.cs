namespace GroundsIce.WebApi.Controllers.Profile
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using GroundsIce.Model.Abstractions.Repositories;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.Model.Entities;
    using GroundsIce.WebApi.Attributes;
    using GroundsIce.WebApi.DTO.Common;
    using Profile = Model.Entities.Profile;

    [AuthorizeApi]
    [RoutePrefix("api/profile")]
    public class ProfileController : ApiController
    {
        private readonly IEnumerable<IProfileInfoValidator> profileInfoValidators;
        private readonly IProfileRepository profileRepository;

        public ProfileController(IEnumerable<IProfileInfoValidator> profileInfoValidators, IProfileRepository profileRepository)
        {
            this.profileInfoValidators = profileInfoValidators ?? throw new ArgumentNullException("profileInfoValidator");
            this.profileRepository = profileRepository ?? throw new ArgumentNullException("profileRepository");
        }

        public enum ValueType
        {
            Success = 1000,
            ProfileNotExists = 2000,
            BadData = 3000,
        }

        [Route("get")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<Value<Profile>> Get(DTO.ProfileRequest dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("dto");
            }

            long requestedUserId = long.Parse(dto.UserId ?? throw new ArgumentNullException("dto.UserId"));
            if (requestedUserId < 0)
            {
                throw new ArgumentOutOfRangeException("requestedUserId");
            }

            Profile profile = await this.profileRepository.GetProfileAsync(requestedUserId);
            if (profile == null)
            {
                return new Value<Profile>((int)ValueType.ProfileNotExists);
            }

            long? ownUserId = this.GetUserIdFromRequest();
            if (!ownUserId.HasValue || (ownUserId.HasValue && ownUserId.Value != requestedUserId))
            {
                RemovePrivateFieldsFrom(profile);
            }

            return new Value<Profile>((int)ValueType.Success) { Payload = profile };
        }

        [Route("set_profile_info")]
        [HttpPost]
        public async Task<Value> SetProfileInfo(DTO.ProfileInfoModel dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("dto");
            }

            if (dto.ProfileInfo == null)
            {
                throw new ArgumentNullException("dto.ProfileInfo");
            }

            long userId = this.GetUserIdFromRequest() ?? throw new ArgumentNullException("userId");
            foreach (var validator in this.profileInfoValidators)
            {
                if (!await validator.ValidateAsync(dto.ProfileInfo))
                {
                    return new Value((int)ValueType.BadData);
                }
            }

            bool updated = await this.profileRepository.SetProfileInfoAsync(userId, dto.ProfileInfo);
            return updated ? new Value((int)ValueType.Success) : new Value((int)ValueType.BadData);
        }

        private static void RemovePrivateFieldsFrom(Profile profile)
        {
            profile.ProfileInfo = profile.ProfileInfo.Where(v => v.IsPublic).ToList();
        }

        private long? GetUserIdFromRequest()
        {
            return this.Request != null && this.Request.Properties.ContainsKey("USER_ID") ?
                (long?)this.Request?.Properties["USER_ID"] :
                null;
        }
    }
}