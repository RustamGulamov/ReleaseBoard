using System;
using ReleaseBoard.Domain.SignalREvents.Abstractions;

namespace ReleaseBoard.Domain.SignalREvents.Job
{
    /// <summary>
    /// События Job.
    /// </summary>
    public record JobSignalREvent : SignalREvent
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="type">Тип события.</param>
        /// <param name="payload">Payload.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        public JobSignalREvent(string type, JobEventPayload payload, string userId)
            : base(type, payload) => UserId = userId;

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public string UserId { get; }
    }
}
