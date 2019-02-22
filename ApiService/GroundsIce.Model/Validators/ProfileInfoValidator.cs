namespace GroundsIce.Model.Validators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.Model.Entities;

    public class ProfileInfoValidator : AbstractValidator<List<ProfileInfoEntry>>, IProfileInfoValidator
    {
        public ProfileInfoValidator()
        {
            this.RuleFor(profileInfo => profileInfo)
                .Must(profileInfo => profileInfo.Count == profileInfo.GroupBy(v => v.Type).Select(group => group.First()).Count());
            this.RuleForEach(profileInfo => profileInfo.Select(e => e.Value))
                .NotEmpty()
                .OverridePropertyName(nameof(ProfileInfoEntry.Value));
            this.RuleForEach(profileInfo => profileInfo)
                .Must(e => !this.TypesMaxLengths.ContainsKey(e.Type) || this.TypesMaxLengths[e.Type] >= e.Value.Length)
                    .When(e => this.TypesMaxLengths != null);
        }

        public IDictionary<ProfileInfoType, int> TypesMaxLengths { get; set; }

        async Task<bool> IProfileInfoValidator.ValidateAsync(List<ProfileInfoEntry> profileInfo)
        {
            return (await this.ValidateAsync(profileInfo)).IsValid;
        }
    }
}
