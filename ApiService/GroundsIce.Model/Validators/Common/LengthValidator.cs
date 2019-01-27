using System;
using System.Threading.Tasks;
using GroundsIce.Model.Abstractions.Validators;

namespace GroundsIce.Model.Validators.Common
{
    public class LengthValidator : IStringValidator
    {
        private int _minLength;
        private int _maxLength;

        public LengthValidator(int minLength, int maxLength)
        {
            _minLength = (minLength > 0) ? minLength : throw new ArgumentOutOfRangeException("minLength");
            _maxLength = (maxLength >= minLength) ? maxLength : throw new ArgumentOutOfRangeException("maxLength");
        }

        public Task<bool> ValidateAsync(string value)
        {
            return Task.FromResult(value != null && _minLength <= value.Length && value.Length <= _maxLength);
        }
    }
}
