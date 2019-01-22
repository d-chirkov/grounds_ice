using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using GroundsIce.Model.Repositories;

namespace GroundsIce.Model.Accounting
{
    /// <summary>
    /// Проверка учётных данных пользователя не прошла
    /// </summary>
    public class CredentialsValidationException : Exception
    {
    }

    public class AccountService
    {
        private List<ICredentialsValidator> usernameValidators_;
        private List<ICredentialsValidator> passwordValidators_;
        private IAccountRepository repo_;

        public AccountService(IAccountRepository repo)
        {
            repo_ = repo ?? throw new ArgumentNullException("repo");
            usernameValidators_ = new List<ICredentialsValidator>();
            passwordValidators_ = new List<ICredentialsValidator>();
        }

        /// <summary>
        /// Добавить валидатор имени пользователя, использовается при регистрации новых пользователей
        /// или изменении учётных данных
        /// </summary>
        /// <param name="validator">Валидатор имени пользователя</param>
        public void AddUsernameValidator(ICredentialsValidator validator) => usernameValidators_.Add(validator);

        /// <summary>
        /// Добавить валидатор пароля пользователя, использовается при регистрации новых пользователей
        /// или изменении учётных данных
        /// </summary>
        /// <param name="validator">Валидатор пароля пользователя</param>
        public void AddPasswordValidator(ICredentialsValidator validator) => passwordValidators_.Add(validator);

        /// <summary>
        /// Регистрация нового пользователя по имени и паролю, с провеедением валидации входных данных
        /// </summary>
        /// <param name="username">Имя (логин) пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <returns>Зарегестрированный пользователь или null, если пользоваель с таким именем уже существует</returns>
        /// <exception cref="CredentialsValidationException">Учётные данные не прошли валидацию</exception>
        public async Task<User> RegisterUserAsync(string username, string password)
        {
            if (!ValidateUsername(username) || !ValidatePassword(password))
            {
                throw new CredentialsValidationException();
            }
            return await repo_.AddNewUserAsync(username, password);
        }

        private bool ValidateUsername(string username) => Validate(username, usernameValidators_);

        private bool ValidatePassword(string password) => Validate(password, passwordValidators_);

        private bool Validate(string what, List<ICredentialsValidator> with) => with.All(v => v.Validate(what));

        public enum Error
        {
            NoError,
            UsernameNotValid,
            PasswordNotValid,
            RepositoryConflict,
            UserNotFound
        }

        /// <summary>
        /// Изменение имени пользователя (логина)
        /// </summary>
        /// <param name="user">Пользователь, для которого необходимо поменять имя (логин)</param>
        /// <param name="username">Новое имя пользователя (логин)</param>
        /// <returns>
        /// Error.NoError - имя пользователя успешно изменено;
        /// Error.RepositoryConflict - имя пользователя уже используется;
        /// Error.UserNotFound - пользователь, для которого необходимо поменять имя, не найден;
        /// Error.UsernameNotValid - новое имя пользоваетя не прошло валидацию
        /// </returns>
        public async Task<Error> ChangeUsernameAsync(User user, string username)
        {
            if (!ValidateUsername(username))
            {
                return Error.UsernameNotValid;
            }
            User userWithUsername = await repo_.ChangeUsernameAsync(user, username);
            return 
                userWithUsername == null ? Error.UserNotFound : 
                !userWithUsername.Equals(user) ? Error.RepositoryConflict : 
                Error.NoError;
        }

        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="user">Пользователь, для которого необходимо поменять пароль</param>
        /// <param name="password">Новый пароль пользователя</param>
        /// <returns>
        /// Error.NoError - пароль успешно изменён;
        /// Error.UserNotFound - пользователь, для которого необходимо поменять пароль, не найден;
        /// Error.PasswordNotValid - новый пароль не прошёл валидацию.
        /// </returns>
        public async Task<Error> ChangePasswordAsync(User user, string password)
        {
            if (!ValidatePassword(password))
            {
                return Error.PasswordNotValid;
            }
            bool changed = await repo_.ChangePasswordAsync(user, password);
            return changed ? Error.NoError : Error.UserNotFound;
        }

        /// <summary>
        /// Получить имя пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Имя пользователя или null, если пользователь не найден</returns>
        public async Task<string> GetUsernameAsync(User user)
        {
            return await repo_.GetUsernameAsync(user);
        }
    }
}
