using System;

namespace ReleaseBoard.Infrastructure.BuildSync
{
    /// <summary>
    /// Настройки очереди для ReleaseBoard.
    /// </summary>
    public class ReleaseBoardRabbitSettings
    {
        /// <summary>
        /// Exchange для запросов ReleaseBoard и BuildSync.
        /// </summary>
        public string ReleaseBoardBuildSyncExchange { get; set; }

        /// <summary>
        /// Queue для событий с BuildSync.
        /// </summary>
        public string BuildSyncEventsQueue { get; set; }

        /// <summary>
        /// Очередь для приема ответов на запросы.
        /// </summary>
        public string BuildSyncResponseQueue { get; set; }
    }
}
