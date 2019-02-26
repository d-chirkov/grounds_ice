namespace GroundsIce.Model.Entities
{
    using System;

    public class Account_OLD
    {
        public Account_OLD(long userId, string login)
        {
            this.UserId = (userId >= 0) ? userId : throw new ArgumentOutOfRangeException("userId");
            this.Login = login ?? throw new ArgumentNullException("login");
        }

        public long UserId { get; private set; }

        public string Login { get; private set; }
    }
}
