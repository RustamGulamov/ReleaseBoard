using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.EventStore.Exceptions
{
    /// <summary>
    /// Is thrown by event store if there were changes since our last version.
    /// </summary>
    public class OptimisticConcurrencyException : Exception
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <param name="actualVersion">Актуальная версия.</param>
        /// <param name="expectedVersion">Ожидаемая версия.</param>
        /// <param name="streamName">Имя потока.</param>
        /// <param name="events">Список события.</param>
        public OptimisticConcurrencyException(
            string message, 
            long actualVersion, 
            long expectedVersion, 
            string streamName,
            IList<Event> events)
            : base(message)
        {
            ActualVersion = actualVersion;
            ExpectedVersion = expectedVersion;
            StreamName = streamName;
            ActualEvents = events;
        }

        /// <summary>
        /// Актальная версия.
        /// </summary>
        public long ActualVersion { get; }

        /// <summary>
        /// Ожидаемая версия.
        /// </summary>
        public long ExpectedVersion { get;  }

        /// <summary>
        /// Имя потока.
        /// </summary>
        public string StreamName { get; }

        /// <summary>
        /// Список события.
        /// </summary>
        public IList<Event> ActualEvents { get; }

        /// <summary>
        /// Создать <see cref="OptimisticConcurrencyException"/>.
        /// </summary>
        /// <param name="actualVersion">Актуальная версия.</param>
        /// <param name="expectedVersion">Ожидаемая версия.</param>
        /// <param name="streamName">Имя потока.</param>
        /// <param name="events">Список события.</param>
        /// <returns><see cref="OptimisticConcurrencyException"/>.</returns>
        public static OptimisticConcurrencyException Create(
            long actualVersion, 
            long expectedVersion, 
            string streamName,
            IList<Event> events)
        {
            string message = $"Expected v{expectedVersion} but found v{actualVersion} in stream '{streamName}'";
            return new OptimisticConcurrencyException(message, actualVersion, expectedVersion, streamName, events);
        }
    }
}
