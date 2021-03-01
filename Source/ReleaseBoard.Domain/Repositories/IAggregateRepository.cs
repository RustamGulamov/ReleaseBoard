using System;
using System.Threading.Tasks;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Repositories
{
    /// <summary>
    /// Интерфейс репозиторий агрегатов.
    /// </summary>
    public interface IAggregateRepository
    {
        /// <summary>
        /// Загружает агрегата по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <typeparam name="TAggregate">Тип агрегатора.</typeparam>
        /// <returns>Агрегат.</returns>
        Task<TAggregate> LoadById<TAggregate>(Guid id) 
            where TAggregate : Aggregate;

        /// <summary>
        /// Обновляет агрегат.
        /// </summary>
        /// <param name="id">Идентификатор агрегата.</param>
        /// <param name="action">Action для обновления.</param>
        /// <typeparam name="TAggregate">Тип агрегатора.</typeparam>
        /// <returns><see cref="Task"/>.</returns>
        Task UpdateById<TAggregate>(Guid id, Func<TAggregate, Task> action) 
            where TAggregate : Aggregate;

        /// <summary>
        /// Добавляет новый агрегат.
        /// </summary>
        /// <typeparam name="TAggregate">Тип агрегатора.</typeparam>
        /// <param name="aggregate">Агрегат.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task Add<TAggregate>(TAggregate aggregate) 
            where TAggregate : Aggregate;
    }
}
