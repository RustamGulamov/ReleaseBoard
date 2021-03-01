using System;
using System.Collections.Generic;

namespace ReleaseBoard.Application.Models
{
    /// <summary>
    /// Модель результата поиска по фильтру.
    /// </summary>
    /// <typeparam name="T">Тип значения модели.</typeparam>
    public class SearchResult<T>
    {
        /// <summary>
        /// Результат.
        /// </summary>
        public IEnumerable<T> Result { get; set; }

        /// <summary>
        /// Тип результата поиска <see cref="SearchResultType"/>.
        /// </summary>
        public SearchResultType Type { get; set; }
    }
}
