namespace GroundsIce.Model.Entities
{
    using System;

    public class ProfileEntry : IEquatable<ProfileEntry>
    {
        public ProfileEntryType Type { get; set; }

        public string Value { get; set; }

        public bool IsPublic { get; set; }

        public bool Equals(ProfileEntry other)
        {
            return other != null && this.Type == other.Type && this.Value == other.Value && this.IsPublic == other.IsPublic;
        }
    }
}
