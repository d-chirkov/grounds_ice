using System;

namespace GroundsIce.Model.Accounting
{
    /// <summary>
    /// Валидатор учётных данныъ пользователя
    /// </summary>
    public interface ICredentialsValidator
    {
        /// <summary>
        /// Проверить учётные данные пользователя
        /// </summary>
        /// <param name="credentials">Учётные данные, представляемые как строка (логин или пароль)</param>
        /// <returns>True, если учётные данные прошли проверку, иначе false</returns>
        bool Validate(string credentials);
    }
}
