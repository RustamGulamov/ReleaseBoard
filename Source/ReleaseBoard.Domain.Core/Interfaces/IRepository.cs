using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReleaseBoard.Domain.Core.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория.
    /// </summary>
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>
    {
        /// <summary>
        /// Добавляет сущность <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">Сущность.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task Add(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Добавляет сущности <paramref name="entities"/>.
        /// </summary>
        /// <param name="entities">Сущности.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task AddMany(TEntity[] entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить сущность по фильтру.
        /// </summary>
        /// <param name="filter">Фильтр.</param>
        /// <param name="entity">Сущность.</param>
        /// <returns><see cref="Task"/>.</returns>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        Task<bool> Update(Expression<Func<TEntity, bool>> filter, TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Удалить сущность.
        /// </summary>
        /// <param name="filter">Фильтр.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task<bool> Delete(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    }
}
