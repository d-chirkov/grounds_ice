using System;

namespace GroundsIce.Model.Accounting
{
    public class CredentialsValidatorException : Exception
    {
        public CredentialsValidatorException(string message) : base(message)
        {
        }
    }
    public interface ICredentialsValidator
    {
        void Validate(Credentials credentials);
    }
}
