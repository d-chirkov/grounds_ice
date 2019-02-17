namespace GroundsIce.Model.Validators.Common
{
    using System;
    using System.Threading.Tasks;
    using GroundsIce.Model.Abstractions.Validators;

    public class LengthValidator : IStringValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        public LengthValidator(int minLength, int maxLength)
        {
            this.minLength = (minLength > 0) ? minLength : throw new ArgumentOutOfRangeException("minLength");
            this.maxLength = (maxLength >= minLength) ? maxLength : throw new ArgumentOutOfRangeException("maxLength");
        }

        public Task<bool> ValidateAsync(string value)
        {
            return Task.FromResult(value != null && this.minLength <= value.Length && value.Length <= this.maxLength);
        }
    }
}
