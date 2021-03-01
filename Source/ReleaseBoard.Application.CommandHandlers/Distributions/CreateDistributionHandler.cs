using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Distributions;
using ReleaseBoard.Domain.Distributions.Commands;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Application.CommandHandlers.Distributions
{
    /// <summary>
    /// Команда на создание дистрибутива.
    /// </summary>
    public class CreateDistributionHandler : CommandHandlerBase<CreateDistribution>
    {
        private readonly IAggregateRepository aggregateRepository;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="aggregateRepository"><see cref="IAggregateRepository"/>.</param>
        /// <param name="logger">Логгер.</param>
        public CreateDistributionHandler(
            IAggregateRepository aggregateRepository,
            ILogger<CreateDistributionHandler> logger
        ) : base(logger)
        {
            this.aggregateRepository = aggregateRepository;
        }

        /// <inheritdoc />
        protected override async Task HandleCore(CreateDistribution command)
        {
            Distribution distribution = new (
                command.Name.Trim(), 
                command.Owners, 
                command.AvailableLifeCycles,
                command.LifeCycleStateRules
                );
            
            foreach (ProjectBinding binding in command.ProjectBindings)
            {
                distribution.AddProjectBinding(binding, command.ActionUser.Sid);
            }

            distribution.AddBuildBindings(command.BuildBindings, command.ActionUser.Sid);

            await aggregateRepository.Add(distribution);
        }
    }
}
