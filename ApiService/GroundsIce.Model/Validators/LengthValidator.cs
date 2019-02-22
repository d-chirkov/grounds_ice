namespace GroundsIce.Model.Validators
{
    using System;
    using FluentValidation;

    public class LengthValidator : AbstractValidator<string>
    {
        public LengthValidator(int minLength, int maxLength)
        {
            if (minLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minLength));
            }

            if (maxLength < minLength)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength));
            }

            this.RuleFor(v => v)
                .NotEmpty()
                .MinimumLength(minLength)
                .MaximumLength(maxLength);
        }
    }
}
