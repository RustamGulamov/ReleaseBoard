using System;
using System.ComponentModel.DataAnnotations;

namespace ReleaseBoard.Application.Models.Filters
{
    /// <summary>
    /// Фильтр для поиска дистрибутива.
    /// </summary>
    public class DistributionsFilter
    {
        /// <summary>
        /// Допустимые дистрибутивы. Если массив пустой, допустимы все репозитории.
        /// </summary>
        [Required]
        public Guid[] SelectedDistributions { get; set; } = Array.Empty<Guid>();
    }
}
