namespace GroundsIce.Model.Entities
{
    using System;

    public class Login : IEquatable<Login>
    {
        private string value;

        public Login(string value)
        {
            this.Value = value ?? throw new ArgumentNullException(value);
        }

        public string Value { get => this.value; set => this.value = value ?? throw new ArgumentNullException(value); }

        public bool Equals(Login other)
        {
            return other != null && other.Value.Equals(this.Value);
        }
    }
}
