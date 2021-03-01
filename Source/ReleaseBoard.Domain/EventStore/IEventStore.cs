using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.EventStore.Exceptions;

namespace ReleaseBoard.Domain.EventStore
{
    /// <summary>
    /// Интерфейс хранилище событий для доступа к потокам событий.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Загружаем все события в потоке.
        /// </summary>
        /// <param name="streamName">Имя потока.</param>
        /// <returns><see cref="EventStream"/>.</returns>
        Task<EventStream> LoadEventStream(string streamName);

        /// <summary>
        /// Загружаем все события в потоке.
        /// </summary>
        /// <param name="id">Идентификатор агрегата.</param>
        /// <returns><see cref="EventStream"/>.</returns>
        Task<EventStream> LoadEventStream<TAggregate>(Guid id);

        /// <summary>
        /// Загружаем в поток событий после версией snapshotVersion.
        /// </summary>
        /// <param name="id">Идентификатор агрегата.</param>
        /// <param name="afterVersion">Версия, после которой будет загружатся события.</param>
        /// <returns><see cref="EventStream"/>.</returns>
        Task<EventStream> LoadEventStreamAfterVersion<TAggregate>(Guid id, long afterVersion);

        /// <summary>
        /// Добавляем события в поток, генерируя исключения <see cref="OptimisticConcurrencyException"/>, если версии событий не совпадают с ожидаемой.
        /// </summary>
        /// <param name="id">Идентификатор агрегата.</param>
        /// <param name="events">Список события.</param>
        /// <param name="expectedVersion">Ожидаемое версия событий. Если новый поток, то номер версии -1.</param>
        /// <exception cref="OptimisticConcurrencyException">Если версии событий не совпадают с ожидаемой.</exception>
        /// <returns><see cref="Task"/>.</returns>
        Task AppendToStream<TAggregate>(Guid id, IReadOnlyCollection<Event> events, long expectedVersion = -1);
    }
}
