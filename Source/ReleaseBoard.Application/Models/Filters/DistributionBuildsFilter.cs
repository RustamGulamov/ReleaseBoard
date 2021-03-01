using System;
using System.ComponentModel.DataAnnotations;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Specifications;

namespace ReleaseBoard.Application.Models.Filters
{
    /// <summary>
    /// Представляет модель для фильтрации сборок дистрибутива.
    /// </summary>
    public class DistributionBuildsFilter
    {
        /// <summary>
        /// Идентификатор дистрибутива.
        /// </summary>
        [Required]
        public Guid DistributionId { get; set; }

        /// <summary>
        /// Период времени создания сборки.
        /// </summary>
        [Required]
        public DateRange CreationDateRange { get; set; }

        /// <summary>
        /// Список тегов для поиска.
        /// </summary>
        public string[] Tags { get; set; } = { };

        /// <summary>
        /// Список суффиксов для поиска.
        /// </summary>
        public string[] Suffixes { get; set; } = { };

        /// <summary>
        /// Логическое условие фильтрации по тегам.
        /// </summary>
        public SelectCondition TagsCondition { get; set; } = SelectCondition.Or;

        /// <summary>
        /// Логическое условие фильтрации по суффиксам.
        /// </summary>
        public SelectCondition SuffixesCondition { get; set; } = SelectCondition.Or;

        /// <summary>
        /// Список состояний сборки <see cref="LifeCycleState"/>.
        /// </summary>
        public LifeCycleState[] LifeCycleStates { get; set; } = { };

        /// <summary>
        /// Список названий сборок для фильтрации.
        /// </summary>
        public string[] Builds { get; set; } = { };
    }
}
