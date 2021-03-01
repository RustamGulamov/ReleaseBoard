using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.EventStore.Exceptions;

namespace ReleaseBoard.Domain.EventStore
{
    /// <summary>
    /// Интерфейс репозиторий истории событий.
    /// Историю можно только добавить и считывать.
    /// </summary>
    public interface IChangesetRepository
    {
        /// <summary>
        /// Добавить новый набор изменений.
        /// </summary>
        /// <param name="name">Название набора изменений.</param>
        /// <param name="events">Список события.</param>
        /// <param name="expectedVersion">Ожидаемая версия.</param>
        /// <exception cref="ChangesetConcurrencyException">Если версии событий не совпадают с ожидаемой.</exception>
        /// <returns><see cref="Task"/>.</returns>
        Task Append(string name, IReadOnlyCollection<Event> events, long expectedVersion = -1);

        /// <summary>
        /// Считывает набор изменений.
        /// </summary>
        /// <param name="name">Название набора изменений.</param>
        /// <param name="afterVersion">Версия, после которой будет считыватся.</param>
        /// <param name="maxCount">The max count.</param>
        /// <returns><see cref="IEnumerable{EventsWithVersion}"/>.</returns>
        Task<IEnumerable<EventsWithVersion>> Read(string name, long afterVersion, int maxCount);
    }
}
