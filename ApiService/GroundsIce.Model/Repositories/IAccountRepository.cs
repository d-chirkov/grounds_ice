using System.Threading.Tasks;
using GroundsIce.Model.Accounting;

namespace GroundsIce.Model.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> GetAccountAsync(User user);
        Task<Account> GetAccountAsync(Credentials credentials);
        Task<Account> AddNewAccountAsync(Credentials credentials);
    }
}
