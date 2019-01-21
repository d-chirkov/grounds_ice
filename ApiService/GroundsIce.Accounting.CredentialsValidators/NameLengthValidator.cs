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

        public bool Validate(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("credentials | credentials.Name");
            }
            return username.Length >= minLength_ && username.Length <= maxLength_;
        }
    }
}
