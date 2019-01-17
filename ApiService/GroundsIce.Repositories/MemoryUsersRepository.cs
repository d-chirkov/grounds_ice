using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GroundsIce.Model;
using GroundsIce.Model.Repositories;

namespace GroundsIce.Repositories
{
    public class MemoryUsersRepository : IUsersRepository
    {
        static SemaphoreSlim access_ = new SemaphoreSlim(1,1);
        static private Dictionary<UInt64, User> users_ = new Dictionary<UInt64, User>();
        static private UInt64 nextUserId_ = 0;

        public async Task<User> AddNewUserAsync(string name, string password)
        {
            var newUser = new User(nextUserId_)
            {
                Name = name,
                Password = password
            };
            await access_.WaitAsync();
            users_[nextUserId_++] = newUser;
            access_.Release();
            return newUser;
        }

        public async Task<User> GetUserByIdAsync(UInt64 key)
        {
            await access_.WaitAsync();
            var user = (from keyUser in users_ where keyUser.Key == key select keyUser.Value).FirstOrDefault();
            access_.Release();
            return user;
        }

        public async Task<User> GetUserByNameAndPasswordAsync(string name, string password)
        {
            await access_.WaitAsync();
            var user = (from keyUser in users_
                        where keyUser.Value.Name == name && keyUser.Value.Password == password
                        select keyUser.Value)
                        .FirstOrDefault();
            access_.Release();
            return user;
        }
    }
}
