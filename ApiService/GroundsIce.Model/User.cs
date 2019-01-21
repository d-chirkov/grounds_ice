using System;

namespace GroundsIce.Model
{
    /// <summary>
    /// Пользователь, представляется уникальным в рамках всей системы
    /// </summary>
    public class User : IEquatable<User>
    {
        public User(long id) => Id = id;

        /// <summary>
        /// Идентификатор пользователя.
        /// Два пользователя с одинаковыми идентификаторами считаются одним и тем же пользователем
        /// </summary>
        public long Id { get; private set; }

        public bool Equals(User other) => (other != null && other.Id == Id);
    }
}
