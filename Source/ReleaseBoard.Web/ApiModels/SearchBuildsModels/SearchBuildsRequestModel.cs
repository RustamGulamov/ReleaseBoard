using System;
using System.ComponentModel.DataAnnotations;

namespace ReleaseBoard.Web.ApiModels.SearchBuildsModels
{
    /// <summary>
    /// Запрос на сканирование сборок в каталоге.
    /// </summary>
    public class SearchBuildsRequestModel
    {
        /// <summary>
        /// Относительный путь к каталогу.
        /// </summary>
        [Required]
        public string Path { get; set; }

        /// <summary>
        /// Тип источника для поиска.
        /// </summary>
        public BuildSourceType SourceType { get; set; }
    }
}
