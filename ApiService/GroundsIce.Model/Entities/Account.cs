using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroundsIce.Model.Entities
{
    public class Account
    {
        public Account(long userId, string login)
        {
			UserId = (userId >= 0) ? userId : throw new ArgumentOutOfRangeException("userId");
            Login = (login != null) ? login : throw new ArgumentNullException("login");
        }

        public long UserId { get; private set; }

        public string Login { get; private set; }
    }
}
