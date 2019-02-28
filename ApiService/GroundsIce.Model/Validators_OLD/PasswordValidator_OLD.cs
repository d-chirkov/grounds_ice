namespace GroundsIce.Model.Validators
{
    using System;
    using System.Threading.Tasks;
    using FluentValidation;
    using GroundsIce.Model.Abstractions.Validators;

    public class PasswordValidator_OLD : AbstractValidator<string>, IPasswordValidator_OLD
    {
        public PasswordValidator_OLD(int minLength, int maxLength)
        {
            this.RuleFor(v => v)
                .NotNull()
                .SetValidator(new LengthValidator_OLD(minLength, maxLength));
        }

        async Task<bool> IStringValidator_OLD.ValidateAsync(string value)
        {
            return (await this.ValidateAsync(value)).IsValid;
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
        }
    }
}
