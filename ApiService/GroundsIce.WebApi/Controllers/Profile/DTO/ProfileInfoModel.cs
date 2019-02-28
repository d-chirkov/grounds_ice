namespace GroundsIce.WebApi.Controllers.Profile.DTO
{
    using System.Collections.Generic;
    using GroundsIce.Model.Entities;

    public class ProfileInfoModel
    {
        public List<ProfileEntry> ProfileInfo { get; set; }
    }
}