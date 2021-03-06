﻿namespace GroundsIce.Model.Entities
{
    using System;

    public class Account
    {
        public Account(long userId, Login login)
        {
            this.UserId = (userId >= 0) ? userId : throw new ArgumentOutOfRangeException("userId");
            this.Login = login ?? throw new ArgumentNullException("login");
        }

        public long UserId { get; private set; }

        public Login Login { get; private set; }
    }
}
