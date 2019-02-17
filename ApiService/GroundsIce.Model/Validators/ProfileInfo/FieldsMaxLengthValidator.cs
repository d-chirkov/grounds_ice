namespace GroundsIce.Model.Validators.ProfileInfo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.Model.Entities;

    public class FieldsMaxLengthValidator : IProfileInfoValidator
    {
        public IDictionary<ProfileInfoType, int> TypesMaxLengths { get; set; }

        public Task<bool> ValidateAsync(List<ProfileInfoEntry> profileInfo)
        {
            if (profileInfo == null)
            {
                throw new ArgumentNullException();
            }

            bool validated =
                this.TypesMaxLengths == null ||
                profileInfo.All(e => !this.TypesMaxLengths.ContainsKey(e.Type) || this.TypesMaxLengths[e.Type] >= e.Value.Length);

            return Task.FromResult(validated);
        }
    }
}
