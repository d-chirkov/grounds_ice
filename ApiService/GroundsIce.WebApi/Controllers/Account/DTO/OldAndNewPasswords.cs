﻿namespace GroundsIce.WebApi.Controllers.Account.DTO
{
    public class OldAndNewPasswords
    {
        public OldAndNewPasswords(string oldPassword, string newPassword)
        {
            this.OldPassword = oldPassword;
            this.NewPassword = newPassword;
        }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
    }
}