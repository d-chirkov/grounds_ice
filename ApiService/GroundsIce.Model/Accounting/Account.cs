using System;

namespace GroundsIce.Model.Accounting
{
    public class Account : IEquatable<Account>
    {
        private User user_;
        private Credentials credentials_;

        public Account(User user, Credentials credentials)
        {
            User = user;
            Credentials = credentials;
        }

        public User User {
            get => user_;
            private set => user_ = value ?? throw new ArgumentNullException("user");
        }

        public Credentials Credentials {
            get => credentials_;
            set => credentials_ = value ?? throw new ArgumentNullException("credentials");
        }

        public bool Equals(Account other)
        {
            return other != null && other.User.Equals(User) && other.Credentials.Equals(Credentials);
        }
    }
}
