namespace GroundsIce.Model.Validators
{
    using System;
    using System.Threading.Tasks;
    using FluentValidation;
    using GroundsIce.Model.Abstractions.Validators;

    public class LoginValidator_OLD : AbstractValidator<string>, ILoginValidator_OLD
    {
        public LoginValidator_OLD(int minLength, int maxLength)
        {
            this.RuleFor(v => v)
                .NotEmpty()
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
