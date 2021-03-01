using System;
using System.Collections.Generic;

namespace ReleaseBoard.Domain.Core
{
    /// <summary>
    /// Базовый класс агрегата.
    /// </summary>
    public abstract class Aggregate
    {
        /// <summary>
        /// Изменения.
        /// </summary>
        private readonly List<Event> changes = new();
        
        /// <summary>
        /// Идентификатор агрегата.
        /// </summary>
        public Guid Id { get; protected set; }
        
        /// <summary>
        /// Фиксация изменений.
        /// </summary>
        public void Commit()
        {
            changes.Clear();
        }

        /// <summary>
        /// Взять незафиксированые изменения.
        /// </summary>
        /// <returns>Список событий.</returns>
        public IReadOnlyCollection<Event> GetUncommitedChanges()
        {
            return changes.AsReadOnly();
        }

        /// <summary>
        /// Применяет преобразования по событию.
        /// </summary>
        /// <param name="e">Событие.</param>
        protected void Apply(Event e)
        {
            Mutate(e);
            changes.Add(e);
        }

        /// <summary>
        /// Выполняет преобразования.
        /// </summary>
        /// <param name="event">Событие.</param>
        protected abstract void Mutate(Event @event);
        
        /// <summary>
        /// Воспроизводит события и применяет мутацию.
        /// </summary>
        /// <param name="events">События.</param>
        protected void Replay(IEnumerable<Event> events)
        {
            foreach (var e in events)
            {
                Mutate(e);
            }
        }

        /// <summary>
        /// Создать объект метаданные.
        /// </summary>
        /// <param name="timestamp">timestamp.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <returns>Метаданные.</returns>
        protected IMetadata CreateMetadata(DateTimeOffset? timestamp = null, string userId = default)
        {
            var metadata = new Metadata { AggregateId = Id, Timestamp = timestamp ?? DateTimeOffset.Now };

            if (!string.IsNullOrWhiteSpace(userId))
            {
                metadata.UserId = userId;
            }

            return metadata;
        }
    }
}
