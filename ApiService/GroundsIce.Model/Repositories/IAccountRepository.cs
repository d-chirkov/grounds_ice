using System;
using System.Threading.Tasks;
using GroundsIce.Model.Accounting;

namespace GroundsIce.Model.Repositories
{
    /// <summary>
    /// Репозиторий аккаунтов
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Получить пользователя по имени и паролю (пройти аутентификацию)
        /// </summary>
        /// <param name="username">Имя пользователя (логин)</param>
        /// <param name="password">Пароль</param>
        /// <returns>Пользователь, соответствующий учётным данным или null, если пользователь не найден</returns>
        Task<User> GetUserAsync(string username, string password);

        /// <summary>
        /// Добавить нового пользователя в репозиторий
        /// </summary>
        /// <param name="username">Имя нового пользователя</param>
        /// <param name="password">Пароль нового пользователя</param>
        /// <returns>Новый пользователь или null, если пользователь с таким именем уже существует</returns>
        Task<User> AddNewUserAsync(string username, string password);

        /// <summary>
        /// Получить имя пользователя (логин) из репозиторий
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Имя пользователя или null, если пользователь не найден</returns>
        Task<string> GetUsernameAsync(User user);

        /// <summary>
        /// Изменить пароль для пользователя
        /// </summary>
        /// <param name="user">Пользователь, для которого необходимо изменить пароль</param>
        /// <param name="newPassword">Новый пароль пользователя</param>
        /// <returns>True, если пароль изменён, и false, если такого пользователя нет</returns>
        Task<bool> ChangePasswordAsync(User user, string newPassword);

        /// <summary>
        /// Изменить имя пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <param name="username">Новое имя пользователя</param>
        /// <returns>Пользователь, который обладает именем, переданным в качестве аргумента. 
        /// Если совпадает с пользователем, переданным в аргументе, значит имя было успешно изменено. 
        /// Если возвращён null, значит пользователь user не был найден и имя пользователя не изменено.
        /// </returns>
        Task<User> ChangeUsernameAsync(User user, string username);
    }
}
