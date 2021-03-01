using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ReleaseBoard.Application.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория для поиска.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    public interface ISearchRepository<TEntity>
    {
        /// <summary>
        /// Поиск по тексту.
        /// </summary>
        /// <param name="value">Текстовый поиск.</param>
        /// <param name="filter">Фильтр данные, если есть.</param>
        /// /// <param name="fields">Поля, где нужно совершить поиск.</param>
        /// <returns>Список найденных объектов.</returns>
        Task<List<TEntity>> TextSearch(string value, Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] fields);
        
        /// <summary>
        /// Создать индексы для полей.
        /// </summary>
        /// <param name="fields">Поля.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task CreateIndex(params Expression<Func<TEntity, object>>[] fields);
    }
}
