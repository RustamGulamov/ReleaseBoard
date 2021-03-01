using System;
using System.Threading.Tasks;
using ReleaseBoard.Domain.Distributions.Commands;
using ReleaseBoard.Web.ApiModels.DistributionModels;

namespace ReleaseBoard.Web.Factories
{
    /// <summary>
    /// Маппер модели <see cref="DistributionModel"/> в команду <see cref="CreateDistribution"/>.
    /// </summary>
    public interface IDistributionCommandMapper
    {
        /// <summary>
        /// Преобразует ui модель в команду.
        /// </summary>
        /// <param name="model">Модель.</param>
        /// <returns>Комманда.</returns>
        Task<UpdateDistribution> MapToUpdateCommand(DistributionModel model);

        /// <summary>
        /// Преобразует ui модель в команду.
        /// </summary>
        /// <param name="model">Модель.</param>
        /// <returns>Комманда.</returns>
        Task<CreateDistribution> MapToCreateCommand(DistributionModel model);
    }
}
