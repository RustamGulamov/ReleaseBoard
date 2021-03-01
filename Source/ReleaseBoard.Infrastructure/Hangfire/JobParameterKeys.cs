using System;

namespace ReleaseBoard.Infrastructure.Hangfire
{
    /// <summary>
    /// Ключи метаданные Job в hangfire.
    /// </summary>
    public class JobParameterKeys
    {
        /// <summary>
        /// Тип событий.
        /// </summary>
        public const string EventType = nameof(EventType);

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public const string UserId = nameof(UserId);
    }
}
