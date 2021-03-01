using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.EventStore
{
    /// <summary>
    /// Модель списка событий с версией.
    /// </summary>
    public class EventsWithVersion
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="version">Версия потока событий.</param>
        /// <param name="events">Данные события.</param>
        public EventsWithVersion(long version, IReadOnlyCollection<Event> events)
        {
            Version = version;
            Events = events;
        }

        /// <summary>
        /// Версия поток событий.
        /// </summary>
        public long Version { get; }

        /// <summary>
        /// Данные события.
        /// </summary>
        public IReadOnlyCollection<Event> Events { get; }
    }
}
