namespace GroundsIce.Model.Abstractions.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GroundsIce.Model.Entities;

    public interface IProfileRepository_OLD
    {
        Task<Profile_OLD> GetProfileAsync(long userId);

        Task<bool> SetProfileInfoAsync(long userId, List<ProfileEntry> profileInfo);
    }
}
