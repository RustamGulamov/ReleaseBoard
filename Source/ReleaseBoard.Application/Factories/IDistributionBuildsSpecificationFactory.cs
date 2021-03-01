using System;
using System.Threading.Tasks;
using ReleaseBoard.Application.Models.Filters;
using ReleaseBoard.Domain.Specifications;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Application.Factories
{
    /// <summary>
    /// Фабрика компоновки спецификаций для фильтрации сборок <see cref="BuildReadModel"/>.
    /// </summary>
    public interface IDistributionBuildsSpecificationFactory
    {
        /// <summary>
        /// Создает выражение для фильтрации сборок по фильтру <see cref="DistributionBuildsFilter"/>.
        /// </summary>
        /// <param name="filter">Фильтр <see cref="DistributionBuildsFilter"/>.</param>
        /// <returns>Выражение для фильтрации.</returns>
        Specification<BuildReadModel> CreateFromFilter(DistributionBuildsFilter filter);
    }
}
