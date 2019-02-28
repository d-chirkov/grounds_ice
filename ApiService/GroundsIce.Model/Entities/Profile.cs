namespace GroundsIce.Model.Entities
{
    // TODO: Надо добавить поле для аватарки (поле типа Avatar, который содержит картинку)
    public class Profile
    {
        public Login Login { get; set; }

        public ProfileEntriesCollection ProfileEntriesCollection { get; set; }
    }
}
