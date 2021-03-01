using System;
using System.Linq;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.Extensions;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.Builds.Mappers;
using ReleaseBoard.Domain.Repositories;

namespace ReleaseBoard.Application.CommandHandlers.Builds
{
    /// <summary>
    /// Обработчик команды обновления билда.
    /// </summary>
    public class UpdateBuildHandler : UpdateBuildBaseHandler<UpdateBuild>
    {
        private readonly IBuildLifeCycleStateMapper lifeCycleStateMapper;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="aggregateRepository"><see cref="IAggregateRepository"/>.</param>
        /// <param name="logger">Logging.</param>
        /// <param name="lifeCycleStateMapper"><see cref="IBuildLifeCycleStateMapper"/>.</param>
        public UpdateBuildHandler(
            IAggregateRepository aggregateRepository,
            ILogger<UpdateBuildHandler> logger,
            IBuildLifeCycleStateMapper lifeCycleStateMapper) :
            base(aggregateRepository, logger)
        {
            this.lifeCycleStateMapper = lifeCycleStateMapper;
        }

        /// <inheritdoc />
        protected override Task UpdateBuild(Build build, UpdateBuild command)
        {
            Logger.LogInformation(
                "Received command UpdateBuild with " +
                $"BuildId: {command.BuildId}, " +
                $"Location: {command.Location}, " +
                $"ReleaseNumber: {command.ReleaseNumber}, " +
                $"Number: {command.Number}, " +
                $"Suffixes: {command.Suffixes?.JoinWith(",")}.");

            LifeCycleState newState = lifeCycleStateMapper.MapFromSuffixes(command.Suffixes.ToArray());

            Logger.LogInformation($"Suffixes were mapped to {newState} for Location: {command.Location}.");

            UpdateBuildCore(build, command, newState);

            Logger.LogInformation($"Build aggregate Id: {command.BuildId} has been updated successfully. NewPath: {command.Location}");

            return Task.CompletedTask;
        }

        private void UpdateBuildCore(Build build, UpdateBuild command, LifeCycleState newState)
        {
            if (build.LifeCycleState != newState)
            {
                Logger.LogWarning("New state of build does not match with current. " +
                    $"Current state: {build.LifeCycleState} -> New state: {newState}. " +
                    $"Current number: {build.Number} -> New number: {command.Number}. " +
                    $"Current location: {build.Location} -> New location: {command.Location}.");

                build.UpdateLifeCycleState(newState, command.ChangeDate);
            }

            build.Update(command.Location, command.ReleaseNumber, command.Number, command.Suffixes, command.ChangeDate);
        }
    }
}
