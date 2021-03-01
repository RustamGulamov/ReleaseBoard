using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ReleaseBoard.Domain.Core.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория только для чтения.
    /// </summary>
    public interface IReadOnlyRepository<TEntity>
    {
        /// <summary>
        /// Возвращает объект.
        /// </summary>
        /// <param name="filter">Фильтр.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task<TEntity> Query(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Возвращает объекты.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task<IList<TEntity>> GetAll(CancellationToken cancellationToken = default);

        /// <summary>
        /// Возвращает объекты.
        /// </summary>
        /// <param name="filter">Фильтр.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task<IList<TEntity>> QueryAll(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Фильтрирует и проекцирует.
        /// </summary>
        /// <typeparam name="TProjection">Тип проекции.</typeparam>
        /// <param name="filter">Фильтр.</param>
        /// <param name="projection">Проекция.</param>
        /// <param name="cancellationToken">Токен.</param>
        /// <returns>Список проецируемых объектов после фильтрации.</returns>
        Task<IList<TProjection>> ProjectManyAsync<TProjection>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProjection>> projection, CancellationToken cancellationToken = default) 
            where TProjection : new();
        
        /// <summary>
        /// Фильтрирует и проекцирует первый совпадающий элемент c фильтром.
        /// </summary>
        /// <typeparam name="TProjection">Тип проекции.</typeparam>
        /// <param name="filter">Фильтр.</param>
        /// <param name="projection">Проекция.</param>
        /// <param name="cancellationToken">Токен.</param>
        /// <returns>Список проецируемых объектов после фильтрации.</returns>
        Task<TProjection> ProjectOne<TProjection>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProjection>> projection, CancellationToken cancellationToken = default) 
            where TProjection : class, new();
        
        /// <summary>
        /// Проверяет, существует ли объект по фильтру.
        /// </summary>
        /// <param name="filter">Фильтр.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task<bool> Any(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    }
}
