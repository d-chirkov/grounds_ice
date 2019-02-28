namespace GroundsIce.Model.Abstractions.Validators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GroundsIce.Model.Entities;

    public interface IProfileInfoValidator_OLD
    {
        Task<bool> ValidateAsync(List<ProfileEntry> profileInfo);
    }
}
