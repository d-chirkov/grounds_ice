using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GroundsIce.Model;
using GroundsIce.Model.Accounting;
using GroundsIce.Model.Repositories;

namespace GroundsIce.Repositories
{
    public class MemoryAccountRepository : IAccountRepository
    {
        class AccountInfo
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
        static SemaphoreSlim access_ = new SemaphoreSlim(1, 1);
        static private Dictionary<long, AccountInfo> accounts_ = new Dictionary<long, AccountInfo>();
        static private long nextUserId_ = 0;

        public MemoryAccountRepository()
        {
            Debug.WriteLine("Repository initiation");
        }
        
        public async Task<User> GetUserAsync(string username, string password)
        {
            await access_.WaitAsync();
            User user = (from account in accounts_
                           where account.Value.Username == username && account.Value.Password == password
                           select new User(account.Key)).FirstOrDefault();
            access_.Release();
            return user;
        }

        public async Task<User> AddNewUserAsync(string username, string password)
        {
            await access_.WaitAsync();
            var newUser = new User(nextUserId_);
            var newAccountInfo = new AccountInfo { Username = username, Password = password };
            if (IsUsernameIsAlreadyUsing(username))
            {
                access_.Release();
                return null;
            }
            accounts_[nextUserId_++] = newAccountInfo;
            access_.Release();
            return newUser;
        }

        private bool IsUsernameIsAlreadyUsing(string username)
        {
            int usersCount = (from account in accounts_
                              where account.Value.Username == username
                              select account).Count();
            return usersCount > 0;
        }

        public async Task<string> GetUsernameAsync(User user)
        {
            await access_.WaitAsync();
            string username = (from account in accounts_
                               where account.Key == user.Id
                               select account.Value.Username).FirstOrDefault();
            access_.Release();
            return username;
        }

        public async Task<bool> ChangePasswordAsync(User user, string password)
        {
            await access_.WaitAsync();
            int accountCount = (from account in accounts_
                                where account.Key == user.Id
                                select account).Count();
            if (accountCount != 1)
            {
                access_.Release();
                return false;
            }
            accounts_[user.Id].Password = password;
            access_.Release();
            return true;
        }

        public async Task<User> ChangeUsernameAsync(User user, string username)
        {
            await access_.WaitAsync();
            int accountCount = (from account in accounts_
                                where account.Key == user.Id
                                select account).Count();
            if (accountCount != 1)
            {
                access_.Release();
                return null;
            }
            int withMatchedUsername = (from account in accounts_
                                where account.Value.Username == username
                                select account).Count();
            if (withMatchedUsername > 0)
            {
                long userId = (from account in accounts_
                               where account.Value.Username == username
                               select account.Key).FirstOrDefault();
                access_.Release();
                return new User(userId);
            }
            accounts_[user.Id].Username = username;
            access_.Release();
            return user;
        }
    }
}
