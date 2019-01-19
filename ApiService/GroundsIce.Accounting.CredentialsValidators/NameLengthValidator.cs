using System;
using GroundsIce.Model.Accounting;

namespace GroundsIce.Accounting.CredentialsValidators
{
    public class NameLengthValidator : ICredentialsValidator
    {
        private int minLength_;
        private int maxLength_;

        public NameLengthValidator(int min, int max)
        {
            if (min < 0 || max < 0 || min > max)
            {
                throw new ArgumentException();
            }
            minLength_ = min;
            maxLength_ = max;
        }

        public void Validate(Credentials credentials)
        {
            if (credentials == null || credentials.Name == null)
            {
                throw new ArgumentNullException("credentials | credentials.Name");
            }
            if (credentials.Name.Length < minLength_)
            {
                throw new CredentialsValidatorException($"Username length must be greater or equals {minLength_}");
            }
            if (credentials.Name.Length > maxLength_)
            {
                throw new CredentialsValidatorException($"Username length must be less or equals {maxLength_}");
            }
        }
    }
}
