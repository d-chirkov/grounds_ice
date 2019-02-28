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
    using Profile_OLD = Model.Entities.Profile_OLD;

    [AuthorizeApi]
    [RoutePrefix("api/profile")]
    public class ProfileController : ApiController
    {
        private readonly IProfileInfoValidator profileInfoValidator;
        private readonly IProfileRepository profileRepository;

        public ProfileController(IProfileInfoValidator profileInfoValidator, IProfileRepository profileRepository)
        {
            this.profileInfoValidator = profileInfoValidator ?? throw new ArgumentNullException("profileInfoValidator");
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
        public async Task<Value<Profile_OLD>> Get(DTO.ProfileRequest dto)
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

            Profile_OLD profile = await this.profileRepository.GetProfileAsync(requestedUserId);
            if (profile == null)
            {
                return new Value<Profile_OLD>((int)ValueType.ProfileNotExists);
            }

            long? ownUserId = this.GetUserIdFromRequest();
            if (!ownUserId.HasValue || (ownUserId.HasValue && ownUserId.Value != requestedUserId))
            {
                RemovePrivateFieldsFrom(profile);
            }

            return new Value<Profile_OLD>((int)ValueType.Success) { Payload = profile };
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
            return
                await this.profileInfoValidator.ValidateAsync(dto.ProfileInfo) &&
                await this.profileRepository.SetProfileInfoAsync(userId, dto.ProfileInfo) ?
                new Value((int)ValueType.Success) :
                new Value((int)ValueType.BadData);
        }

        private static void RemovePrivateFieldsFrom(Profile_OLD profile)
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