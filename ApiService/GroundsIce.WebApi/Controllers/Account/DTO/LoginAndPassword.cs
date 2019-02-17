namespace GroundsIce.WebApi.Controllers.Account.DTO
{
    public class LoginAndPassword
    {
        public LoginAndPassword(string login, string password)
        {
            this.Login = login;
            this.Password = password;
        }

        public string Login { get; set; }

        public string Password { get; set; }
    }
}