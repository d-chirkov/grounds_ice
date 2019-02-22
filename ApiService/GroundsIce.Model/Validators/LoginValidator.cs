namespace GroundsIce.Model.Validators
{
    using System;
    using System.Threading.Tasks;
    using FluentValidation;
    using GroundsIce.Model.Abstractions.Validators;

    public class LoginValidator : AbstractValidator<string>, ILoginValidator
    {
        public LoginValidator(int minLength, int maxLength)
        {
            this.RuleFor(v => v)
                .NotEmpty()
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
