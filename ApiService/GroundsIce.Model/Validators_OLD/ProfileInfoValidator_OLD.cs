namespace GroundsIce.Model.Validators
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using GroundsIce.Model.Abstractions.Validators;
    using GroundsIce.Model.Entities;

    public class ProfileInfoValidator_OLD : AbstractValidator<List<ProfileEntry>>, IProfileInfoValidator_OLD
    {
        public ProfileInfoValidator_OLD()
        {
            this.RuleFor(profileInfo => profileInfo)
                .Must(profileInfo => profileInfo.Count == profileInfo.GroupBy(v => v.Type).Select(group => group.First()).Count());
            this.RuleForEach(profileInfo => profileInfo.Select(e => e.Value))
                .NotEmpty()
                .OverridePropertyName(nameof(ProfileEntry.Value));
            this.RuleForEach(profileInfo => profileInfo)
                .Must(e => !this.TypesMaxLengths.ContainsKey(e.Type) || this.TypesMaxLengths[e.Type] >= e.Value.Length)
                    .When(e => this.TypesMaxLengths != null);
        }

        public IDictionary<ProfileEntryType, int> TypesMaxLengths { get; set; }

        async Task<bool> IProfileInfoValidator_OLD.ValidateAsync(List<ProfileEntry> profileInfo)
        {
            return (await this.ValidateAsync(profileInfo)).IsValid;
        }
    }
}
