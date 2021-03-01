using System;
using System.Collections.Generic;

namespace ReleaseBoard.Domain.Builds.Mappers
{
    /// <summary>
    /// Маппер для <see cref="LifeCycleState"/>.
    /// </summary>
    public interface IBuildLifeCycleStateMapper
    {
        /// <summary>
        /// Мапирует список суффиксов к <see cref="LifeCycleState"/>.
        /// </summary>
        /// <param name="suffixes">Список суффиксов.</param>
        /// <returns><see cref="LifeCycleState"/>.</returns>
        LifeCycleState MapFromSuffixes(IEnumerable<string> suffixes);

        /// <summary>
        /// Мапирует из <see cref="LifeCycleState"/> к соответствующему суффиксу.
        /// </summary>
        /// <param name="lifeCycleState"><see cref="LifeCycleState"/>.</param>
        /// <returns>Соответствующий суффикс.</returns>
        string MapToSuffix(LifeCycleState lifeCycleState);
    }
}
