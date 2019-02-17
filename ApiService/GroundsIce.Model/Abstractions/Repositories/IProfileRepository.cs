namespace GroundsIce.Model.Abstractions.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GroundsIce.Model.Entities;

    public interface IProfileRepository
    {
        Task<Profile> GetProfileAsync(long userId);

        Task<bool> SetProfileInfoAsync(long userId, List<ProfileInfoEntry> profileInfo);
    }
}
