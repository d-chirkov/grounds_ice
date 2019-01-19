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
    public class MemoryUsersRepository : IAccountRepository
    {
        static SemaphoreSlim access_ = new SemaphoreSlim(1,1);
        static private Dictionary<UInt64, Account> accounts_ = new Dictionary<UInt64, Account>();
        static private UInt64 nextUserId_ = 0;

        public MemoryUsersRepository()
        {
            Debug.WriteLine("Repository initiation");
        }
        public async Task<Account> AddNewAccountAsync(Credentials credentials)
        {
            await access_.WaitAsync();
            var newUser = new User(nextUserId_);
            var newAccount = new Account(newUser, credentials);
            if (IsUserWithNameExists(credentials.Name))
            {
                access_.Release();
                throw new CredentialsValidatorException($"User with name {credentials.Name} already exists");
            }
            accounts_[nextUserId_++] = newAccount;
            access_.Release();
            return newAccount;
        }

        public async Task<Account> GetAccountAsync(User user)
        {
            await access_.WaitAsync();
            var account = (from keyAccount in accounts_ where keyAccount.Key == user.Id select keyAccount.Value).FirstOrDefault();
            access_.Release();
            return account;
        }

        public async Task<Account> GetAccountAsync(Credentials credentials)
        {
            await access_.WaitAsync();
            var account = (from keyUser in accounts_
                        where keyUser.Value.Credentials.Equals(credentials)
                        select keyUser.Value)
                        .FirstOrDefault();
            access_.Release();
            return account;
        }

        private bool IsUserWithNameExists(string name)
        {
            var user = (from keyUser in accounts_
                        where keyUser.Value.Credentials.Name == name
                        select keyUser.Value)
                        .FirstOrDefault();
            return user != null;
        }
    }
}
