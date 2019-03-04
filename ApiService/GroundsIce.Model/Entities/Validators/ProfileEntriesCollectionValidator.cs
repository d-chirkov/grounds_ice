namespace GroundsIce.Model.Entities.Validators
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using GroundsIce.Model.Entities;

    public class ProfileEntriesCollectionValidator : AbstractValidator<ProfileEntriesCollection>
    {
        public ProfileEntriesCollectionValidator()
        {
            this.RuleFor(c => c)
                .Must(c => !this.IsCollectionContainsRepeatingEntryTypes(c));
            this.RuleForEach(c => c.Select(e => e.Value))
                .NotEmpty()
                .OverridePropertyName(nameof(ProfileEntry.Value));
            this.RuleForEach(c => c.Select(e => e.Type))
                .IsInEnum()
                .OverridePropertyName(nameof(ProfileEntry.Type));
            this.RuleForEach(c => c)
                .Must(e => !this.TypesMaxLengths.ContainsKey(e.Type) || this.TypesMaxLengths[e.Type] >= e.Value.Length)
                    .When(e => this.TypesMaxLengths != null);
        }

        public IDictionary<ProfileEntryType, int> TypesMaxLengths { get; set; }

        private bool IsCollectionContainsRepeatingEntryTypes(ProfileEntriesCollection collection)
        {
            return collection.Count() != collection.GroupBy(v => v.Type).Select(group => group.First()).Count();
        }
    }
}
