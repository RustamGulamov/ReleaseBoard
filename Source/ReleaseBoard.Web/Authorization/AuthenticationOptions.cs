using System;

namespace ReleaseBoard.Web.Authorization
{
    /// <summary>
    /// Свойства для аутентификации.
    /// </summary>
    public class AuthenticationOptions
    {
        /// <summary>
        /// Адрес сервиса аутентификации.
        /// </summary>
        public string Authority { get; set; } = string.Empty;

        /// <summary>
        /// Аудитория.
        /// </summary>
        public string Audience { get; set; } = string.Empty;
    }
}
