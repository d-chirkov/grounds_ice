using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GroundsIce.Model.Repositories;

namespace GroundsIce.Model.Accounting
{
    public class Registrator
    {
        private IEnumerable<ICredentialsValidator> validators_;
        private IAccountRepository repo_;

        public Registrator(IAccountRepository repo, IEnumerable<ICredentialsValidator> validators)
        {
            repo_ = repo ?? throw new ArgumentNullException("repo");
            validators_ = validators;
        }
        
        /// <exception cref="CredentialsValidatorException"></exception>
        /// <returns>Account, related to registered user with appointed id</returns>
        public async Task<Account> RegisterAsync(Credentials credentials)
        {
            if (validators_ != null)
            {
                foreach (var validator in validators_)
                {
                    validator.Validate(credentials);
                }
            }
            return await repo_.AddNewAccountAsync(credentials);
        }
    }
}
