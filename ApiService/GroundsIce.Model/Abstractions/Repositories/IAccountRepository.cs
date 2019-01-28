using System;
using System.Threading.Tasks;
using GroundsIce.Model.Entities;

namespace GroundsIce.Model.Abstractions.Repositories
{
	public class UserIdNotFoundException : Exception
	{
	}

    public interface IAccountRepository
    {
        Task<Account> CreateAccountAsync(string login, string password);
        Task<Account> GetAccountAsync(string login, string password);
        Task<Account> GetAccountAsync(long userId);
        Task<bool> ChangeLoginAsync(long userId, string newLogin);
        Task<bool> ChangePasswordAsync(long userId, string oldPassword, string newPassword);
    }
}
