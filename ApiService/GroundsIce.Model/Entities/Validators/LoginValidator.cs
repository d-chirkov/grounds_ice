namespace GroundsIce.Model.Entities.Validators
{
    using System;
    using FluentValidation;

    public class LoginValidator : AbstractValidator<Login>
    {
        public LoginValidator(int minLength, int maxLength)
        {
            if (minLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minLength));
            }

            if (maxLength < minLength)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength));
            }

            this.RuleFor(v => v.Value)
                .NotEmpty()
                .MinimumLength(minLength)
                .MaximumLength(maxLength);
        }
    }
}
