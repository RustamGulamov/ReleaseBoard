using System;

namespace ReleaseBoard.Application.Models
{
    /// <summary>
    /// Перечисление типов результатов поиска.
    /// </summary>
    public enum SearchResultType
    {
        /// <summary>
        /// Тип результата поиска - сборки.
        /// </summary>
        Builds = 0,

        /// <summary>
        /// Тип результата поиска - сборки.
        /// </summary>
        Distributions = 1
    }
}
