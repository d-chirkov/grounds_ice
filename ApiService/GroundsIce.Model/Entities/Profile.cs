namespace GroundsIce.Model.Entities
{
    // TODO: Надо добавить поле для аватарки (поле типа Avatar, который в свою очередь поле с картинкой)
    public class Profile
    {
        public Login Login { get; set; }

        public ProfileEntriesCollection ProfileEntriesCollection { get; set; }
    }
}
