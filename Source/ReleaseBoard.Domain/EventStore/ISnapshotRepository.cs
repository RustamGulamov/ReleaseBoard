using System;
using System.Threading.Tasks;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.EventStore
{
    /// <summary>
    /// Интерфейс репозиторий для снепшота состояния агрегата.
    /// </summary>
    public interface ISnapshotRepository
    {
        /// <summary>
        /// Получить снепшот агрегата по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор агрегата.</param>
        /// <returns>Агрегат с версией.</returns>
        Task<(TAggregate, long)> GetSnapshotById<TAggregate>(Guid id) 
            where TAggregate : Aggregate;

        /// <summary>
        /// Сохранить снепшот для агрегата.
        /// </summary>
        /// <typeparam name="TAggregate">Тип агрегата.</typeparam>
        /// <param name="aggregate">Агрегат.</param>
        /// <param name="version">Версия.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task SaveSnapshot<TAggregate>(TAggregate aggregate, long version) 
            where TAggregate : Aggregate;
    }
}
