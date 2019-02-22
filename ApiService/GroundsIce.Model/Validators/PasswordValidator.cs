namespace GroundsIce.Model.Validators
{
    using System;
    using System.Threading.Tasks;
    using FluentValidation;
    using GroundsIce.Model.Abstractions.Validators;

    public class PasswordValidator : AbstractValidator<string>, IPasswordValidator
    {
        public PasswordValidator(int minLength, int maxLength)
        {
            this.RuleFor(v => v)
                .NotNull()
                .SetValidator(new LengthValidator(minLength, maxLength));
        }

        async Task<bool> IStringValidator.ValidateAsync(string value)
        {
            return (await this.ValidateAsync(value)).IsValid;
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
        }
    }
}
