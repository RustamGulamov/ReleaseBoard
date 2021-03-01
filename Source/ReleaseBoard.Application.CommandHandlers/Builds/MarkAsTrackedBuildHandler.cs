using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.Repositories;

namespace ReleaseBoard.Application.CommandHandlers.Builds
{
    /// <summary>
    /// Обработчик команды удаления сборки.
    /// </summary>
    public class MarkAsTrackedBuildHandler : UpdateBuildBaseHandler<MarkAsTrackedBuild>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="aggregateRepository"><see cref="IAggregateRepository"/>.</param>
        /// <param name="logger">Logging.</param>
        public MarkAsTrackedBuildHandler(
            IAggregateRepository aggregateRepository,
            ILogger<MarkAsTrackedBuildHandler> logger) :
            base(aggregateRepository, logger)
        {
        }

        /// <inheritdoc />
        protected override Task UpdateBuild(Build build, MarkAsTrackedBuild command)
        {
            Logger.LogInformation($"Received command MarkAsTracked: BuildId: {command.BuildId}, NewDistributionId: {command.DistributionId}.");

            build.MarkAsTracked(command.DistributionId, command.MarkDate);

            Logger.LogInformation($"Build aggregate Id: {command.BuildId} has been restored successfully. NewDistributionId: {command.DistributionId}.");
            return Task.CompletedTask;
        }
    }
}
