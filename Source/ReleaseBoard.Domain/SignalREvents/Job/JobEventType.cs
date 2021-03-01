using System;

namespace ReleaseBoard.Domain.SignalREvents.Job
{
    /// <summary>
    /// Типы события для Job.
    /// </summary>
    public static class JobEventType
    {
        /// <summary>
        /// Парсинг билдов.
        /// </summary>
        public static readonly string BuildParse = $"@job/{nameof(BuildParse)}";
    }
}
