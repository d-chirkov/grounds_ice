using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GroundsIce.Model.Abstractions.Repositories;
using GroundsIce.Model.Entities;

namespace GroundsIce.Model.Repositories
{
    public class InMemoryAccountRepository : IAccountRepository
    {
        public class LoginPassword : IEquatable<LoginPassword>
        {
            public string Login { get; set; }
            public string Password { get; set; }

            public bool Equals(LoginPassword other) => (other != null && other.Login == Login && other.Password == Password);
        }

        private static SemaphoreSlim _access = new SemaphoreSlim(1, 1);
        private static Dictionary<long, LoginPassword> _storage = new Dictionary<long, LoginPassword>();
        private static long _nextUserId = 0;

        public static Dictionary<long, LoginPassword> GetStorage() => _storage;
        public static void Clear() {
            _nextUserId = 0;
            _storage = new Dictionary<long, LoginPassword>();
            _access = new SemaphoreSlim(1, 1);
        }

        public async Task<Account> CreateAccountAsync(string login, string password)
        {
            if (login == null) throw new ArgumentNullException("login");
            if (password == null) throw new ArgumentNullException("password");

            await _access.WaitAsync();
            if (_storage.Values.FirstOrDefault(e => e.Login == login) != null)
            {
                _access.Release();
                return null;
            }
            long newUserId = _nextUserId++;
            _storage[newUserId] = new LoginPassword() { Login = login, Password = password };
            _access.Release();
            return new Account(newUserId, login);
        }

        public async Task<Account> GetAccountAsync(string login, string password)
        {
            if (login == null) throw new ArgumentNullException("login");
            if (password == null) throw new ArgumentNullException("password");
            var loginPassword = new LoginPassword { Login = login, Password = password };
            await _access.WaitAsync();
            Account account = (
                from row in _storage
                where row.Value.Equals(loginPassword)
                select new Account(row.Key, row.Value.Login)).FirstOrDefault();
            _access.Release();
            return account;
        }

        public async Task<Account> GetAccountAsync(long userId)
        {
            if (userId < 0) throw new ArgumentOutOfRangeException("userId");
            await _access.WaitAsync();
            Account account = (
                from row in _storage
                where row.Key == userId
				select new Account(row.Key, row.Value.Login)).FirstOrDefault();
            _access.Release();
            return account;
        }

        public async Task<bool> ChangeLoginAsync(long userId, string newLogin)
        {
            if (userId < 0) throw new ArgumentOutOfRangeException("userId");
            if (newLogin == null) throw new ArgumentNullException("newLogin");
            await _access.WaitAsync();
            if (_storage.Count(e => e.Value.Login == newLogin) > 0)
            {
                _access.Release();
                return false;
            }
			if (!_storage.ContainsKey(userId))
			{
				_access.Release();
				throw new UserIdNotFoundException();
			}
            _storage[userId].Login = newLogin;
            _access.Release();
            return true;
        }

        public async Task ChangePasswordAsync(long userId, string newPassword)
        {
            if (userId < 0) throw new ArgumentOutOfRangeException("userId");
            if (newPassword == null) throw new ArgumentNullException("newPassword");
            await _access.WaitAsync();
            if (!_storage.ContainsKey(userId))
            {
                _access.Release();
				throw new UserIdNotFoundException();
            }
            _storage[userId].Password = newPassword;
            _access.Release();
            return;
        }

    }
}
