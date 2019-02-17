namespace GroundsIce.Model.Entities
{
    using System.Collections.Generic;

    public class Profile
    {
        public string Login { get; set; }

        public string Avatar { get; set; }

        public List<ProfileInfoEntry> ProfileInfo { get; set; }
    }
}
