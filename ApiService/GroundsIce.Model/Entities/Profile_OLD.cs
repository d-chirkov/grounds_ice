namespace GroundsIce.Model.Entities
{
    using System.Collections.Generic;

    public class Profile_OLD
    {
        public string Login { get; set; }

        public string Avatar { get; set; }

        public List<ProfileEntry> ProfileInfo { get; set; }
    }
}
