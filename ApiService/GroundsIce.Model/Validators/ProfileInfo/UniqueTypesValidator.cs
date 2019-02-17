namespace GroundsIce.Model.Validators.ProfileInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.Model.Entities;

    public class UniqueTypesValidator : IProfileInfoValidator
    {
        public Task<bool> ValidateAsync(List<ProfileInfoEntry> profileInfo)
        {
            if (profileInfo == null)
            {
                throw new ArgumentNullException("profileInfo");
            }

            return Task.FromResult(profileInfo.Count == profileInfo.GroupBy(v => v.Type).Select(group => group.First()).Count());
        }
    }
}
