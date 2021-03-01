using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Distributions;
using ReleaseBoard.Domain.Distributions.Commands;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.Domain.Services;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Application.CommandHandlers.Distributions
{
    /// <summary>
    /// Команда на создание дистрибутива.
    /// </summary>
    public class UpdateDistributionHandler : CommandHandlerBase<UpdateDistribution>
    {
        private readonly IAggregateRepository aggregateRepository;
        private readonly ICollectionsComparer collectionsComparer;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="aggregateRepository"><see cref="IAggregateRepository"/>.</param>
        /// <param name="collectionsComparer">Сравниватель коллекций.</param>
        /// <param name="logger">Логгер.</param>
        public UpdateDistributionHandler(
            IAggregateRepository aggregateRepository,
            ICollectionsComparer collectionsComparer,
            ILogger<UpdateDistributionHandler> logger
        ) : base(logger)
        {
            this.aggregateRepository = aggregateRepository;
            this.collectionsComparer = collectionsComparer;
        }

        /// <inheritdoc />
        protected override async Task HandleCore(UpdateDistribution command)
        {
            await aggregateRepository.UpdateById<Distribution>(command.Id,
                 distribution =>
                 {
                     distribution.UpdateName(command.Name.Trim());

                     UpdateOwners(command, distribution);
                     UpdateAvailableLifeCycles(command, distribution);
                     UpdateLifeCycleRules(command, distribution);
                     UpdateBuildBindings(command, distribution);
                     UpdateProjectBindings(command, distribution);
                     
                     return Task.CompletedTask;
                 });
        }

        private void UpdateOwners(UpdateDistribution command, Distribution distribution)
        {
            (IList<User> addedOwners, IList<User> removedOwners) =
                collectionsComparer
                    .Compare(distribution.Owners, command.Owners);

            distribution.AddOwners(addedOwners);
            distribution.RemoveOwners(removedOwners);
        }

        private void UpdateAvailableLifeCycles(UpdateDistribution command, Distribution distribution)
        {
            (IList<LifeCycleState> newStates, IList<LifeCycleState> oldStates) =
                collectionsComparer
                    .Compare(distribution.AvailableLifeCycles, command.AvailableLifeCycles);

            distribution.AddAvailableLifeCycleStates(newStates);
            distribution.RemoveAvailableLifeCycleStates(oldStates);
        }

        private void UpdateLifeCycleRules(UpdateDistribution command, Distribution distribution)
        {
            (IList<string> newRules, IList<string> oldRules) =
                collectionsComparer
                    .Compare(distribution.LifeCycleStateRules, command.LifeCycleStateRules);

            distribution.AddLifeCycleStateRules(newRules);
            distribution.RemoveLifeCycleStateRules(oldRules);
        }

        private void UpdateBuildBindings(UpdateDistribution command, Distribution distribution)
        {
            (IList<BuildsBinding> newBindings, IList<BuildsBinding> bindingsToDelete) =
                collectionsComparer
                    .Compare(distribution.BindingToBuilds.Keys, command.BuildBindings);
            
            distribution.AddBuildBindings(newBindings, command.ActionUser.Sid);
            distribution.RemoveBuildBinding(bindingsToDelete, command.ActionUser.Sid);
        }

        private void UpdateProjectBindings(UpdateDistribution command, Distribution distribution)
        {
            (IList<ProjectBinding> newBindings, IList<ProjectBinding> bindingsToDelete) =
                collectionsComparer
                    .Compare(distribution.ProjectBindings, command.ProjectBindings);

            foreach (ProjectBinding binding in bindingsToDelete)
            {
                distribution.RemoveProjectBinding(binding, command.ActionUser.Sid);
            }

            foreach (ProjectBinding binding in newBindings)
            {
                distribution.AddProjectBinding(binding, command.ActionUser.Sid);
            }
        }
    }
}
