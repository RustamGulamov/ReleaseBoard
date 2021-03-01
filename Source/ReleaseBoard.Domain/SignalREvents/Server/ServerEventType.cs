using System;

namespace ReleaseBoard.Domain.SignalREvents.Server
{
    /// <summary>
    /// Типы события сервера.
    /// </summary>
    public class ServerEventType
    {
        /// <summary>
        /// Сборка обновлена.
        /// </summary>
        public const string BuildUpdated = nameof(BuildUpdated);

        /// <summary>
        /// Сборка создана.
        /// </summary>
        public const string BuildCreated = nameof(BuildCreated);

        /// <summary>
        /// Дистрибутив создан.
        /// </summary>
        public const string DistributionCreated = nameof(DistributionCreated);

        /// <summary>
        /// Дистрибутив обновлен.
        /// </summary>
        public const string DistributionUpdated = nameof(DistributionUpdated);
    }
}
