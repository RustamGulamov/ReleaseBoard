using System;

namespace ReleaseBoard.Web.ApiModels.SearchBuildsModels
{
    /// <summary>
    /// Результат сканирования сборок в каталоге.
    /// </summary>
    public class SearchBuildsResultModel
    {
        /// <summary>
        /// Идентификатор шаблона соответствия.
        /// </summary>
        public string PatternMatchId { get; set; }
        
        /// <summary>
        /// Количество найденных сборок.
        /// </summary>
        public int BuildsCount { get; set; }
    }
}
