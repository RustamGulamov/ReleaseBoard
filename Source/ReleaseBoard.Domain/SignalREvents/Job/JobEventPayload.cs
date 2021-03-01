using System;

namespace ReleaseBoard.Domain.SignalREvents.Job
{
    /// <summary>
    /// Данные события job.
    /// </summary>
    public sealed record JobEventPayload(string Id, int Percent)
    {
        /// <summary>
        /// Завершилась ли задача.
        /// </summary>
        public bool IsCompleted => Percent == 100;
    }
}
