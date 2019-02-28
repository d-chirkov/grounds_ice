namespace GroundsIce.Model.Entities.Validators
{
    using System;
    using FluentValidation;

    public class PasswordValidator : AbstractValidator<Password>
    {
        public PasswordValidator(int minLength, int maxLength)
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
