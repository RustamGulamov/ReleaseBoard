using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.EventStore
{
    /// <summary>
    /// Модель поток событий.
    /// </summary>
    public class EventStream
    {
        /// <summary>
        /// Актуальная версия потока событий.
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// Все события в потоке.
        /// </summary>
        public IReadOnlyList<Event> Events { get; set; } = new List<Event>();
    }
}
