using System;

namespace GroundsIce.Model.Accounting
{
    public class Credentials : IEquatable<Credentials>
    {
        private string name_;
        private string password_;

        public Credentials(string name, string password)
        {
            Name = name;
            Password = password;
        }

        public string Name
        {
            get => name_;
            set => name_ = value ?? throw new ArgumentNullException("name");
        }

        public string Password
        {
            get => password_;
            set => password_ = value ?? throw new ArgumentNullException("password");
        }

        public bool Equals(Credentials other)
        {
            return other != null && other.Name == Name && other.Password == Password;
        }
    }
}
