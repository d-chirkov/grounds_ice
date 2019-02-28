namespace GroundsIce.Model.Abstractions.Validators
{
    using System.Threading.Tasks;
    using GroundsIce.Model.Entities;

    public interface IBorrowOrderValidator_OLD
    {
        Task<bool> ValidateAsync(BorrowOrder borrowOrder);
    }
}
