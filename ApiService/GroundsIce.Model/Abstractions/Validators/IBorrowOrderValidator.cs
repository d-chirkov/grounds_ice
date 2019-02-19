namespace GroundsIce.Model.Abstractions.Validators
{
    using System.Threading.Tasks;
    using GroundsIce.Model.Entities;

    public interface IBorrowOrderValidator
    {
        Task<bool> ValidateAsync(BorrowOrder borrowOrder);
    }
}
