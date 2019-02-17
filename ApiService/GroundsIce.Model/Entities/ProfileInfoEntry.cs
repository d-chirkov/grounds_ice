namespace GroundsIce.Model.Entities
{
    using System;

    public class ProfileInfoEntry : IEquatable<ProfileInfoEntry>
    {
        public ProfileInfoType Type { get; set; }

        public string Value { get; set; }

        public bool IsPublic { get; set; }

        public bool Equals(ProfileInfoEntry other)
        {
            return other != null && this.Type == other.Type && this.Value == other.Value && this.IsPublic == other.IsPublic;
        }
    }
}
