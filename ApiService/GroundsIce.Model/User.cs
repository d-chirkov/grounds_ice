using System;

namespace GroundsIce.Model
{
    public class User : IEquatable<User>
    {
        public User(UInt64 id) => Id = id;
        public UInt64 Id { get; set; }

        public bool Equals(User other)
        {
            return other.Id == Id;
        }
    }
}
