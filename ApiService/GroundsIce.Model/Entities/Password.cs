namespace GroundsIce.Model.Entities
{
    using System;

    public class Password : IEquatable<Password>
    {
        public Password(string value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; private set; }

        public bool Equals(Password other)
        {
            return other != null && other.Value.Equals(this.Value);
        }
    }
}
