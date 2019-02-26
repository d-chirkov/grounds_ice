namespace GroundsIce.Model.Entities
{
    using System;

    public class Login : IEquatable<Login>
    {
        public Login(string value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; private set; }

        public bool Equals(Login other)
        {
            return other != null && other.Value.Equals(this.Value);
        }
    }
}
