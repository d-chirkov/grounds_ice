namespace GroundsIce.Model.Abstractions.Repositories
{
    using System.Threading.Tasks;
    using GroundsIce.Model.Entities;

    public interface IProfileRepository
    {
        Task<Profile> GetProfileAsync(long userId);

        Task<bool> SetProfileEntriesCollectionAsync(long userId, ProfileEntriesCollection profileEntriesCollection);
    }
}
