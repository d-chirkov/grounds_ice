using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroundsIce.Model;

namespace GroundsIce.Model.Repositories
{
    public interface IUsersRepository
    {
        Task<User> GetUserByIdAsync(UInt64 key);
        Task<User> GetUserByNameAndPasswordAsync(string name, string password);
        Task<User> AddNewUserAsync(string name, string password);
    }
}
