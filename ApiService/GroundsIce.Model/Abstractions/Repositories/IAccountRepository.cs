namespace GroundsIce.Model.Abstractions.Repositories
{
    using System;
    using System.Threading.Tasks;
    using GroundsIce.Model.Entities;

    public interface IAccountRepository
    {
        /// <summary>
        /// Создаёт новый аккаунт с указанными учётными данными (логином и паролем)
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Созданный аккаунт или null, если указанный логин уже используется</returns>
        Task<Account_OLD> CreateAccountAsync(Login login, Password password);

        /// <summary>
        /// Возвращает аккаунт пользователя по логину и парою
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Аккаунт, соответствующий логину и паролю или null, если нет такого аккаунта (или пароль не верен)</returns>
        Task<Account_OLD> GetAccountAsync(Login login, Password password);

        /// <summary>
        /// Получить аккаунт, соответствующий идентификатору пользователя userId
        /// </summary>
        /// <param name="userId">Идентификатором пользователя</param>
        /// <returns>Аккаунт, соответствующий идентификатору пользователя или null, если такого аккаунта нет</returns>
        Task<Account_OLD> GetAccountAsync(long userId);

        /// <summary>
        /// Поменять логин аккаунта, соответствующего пользователю с идентификатором userId
        /// </summary>
        /// <param name="userId">Идентификатором пользователя</param>
        /// <param name="newLogin">Новый логин</param>
        /// <returns>true - логин успешно изменён; false - логин уже используется и не может быть установлен</returns>
        /// <exception cref="Exception">Не удалось изменить логин по причинам, отличным от конфликта логинов</exception>
        Task<bool> ChangeLoginAsync(long userId, Login newLogin);

        /// <summary>
        /// Изменить пароль аккаунта, соответствующего пользователю с идентификатором userId
        /// </summary>
        /// <param name="userId">Идентификатором пользователя</param>
        /// <param name="oldPassword">Текущий действительный пароль</param>
        /// <param name="newPassword">Новый пароль</param>
        /// <returns>true - пароль успешно изменён; false - старый пароль неверен или пользователя с таким аккаунтом нет</returns>
        Task<bool> ChangePasswordAsync(long userId, Password oldPassword, Password newPassword);
    }
}
