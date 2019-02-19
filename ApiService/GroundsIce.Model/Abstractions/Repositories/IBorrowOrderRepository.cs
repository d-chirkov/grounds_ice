namespace GroundsIce.Model.Abstractions.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GroundsIce.Model.Entities;

    public interface IBorrowOrderRepository
    {
        Task CreateBorrowOrder(long userId, BorrowOrder borrowOrder);

        Task<bool> DeleteBorrowOrder(long borrowOrderId);

        Task<IList<BorrowOrder>> GetBorrowOrders(long userId);

        Task<BorrowOrder> GetDetailedBorrowOrder(long borrowOrderId);

        // TODO: add methods for filtering by fields (may be it's necessary to create special struct with filtering fields)
    }
}
