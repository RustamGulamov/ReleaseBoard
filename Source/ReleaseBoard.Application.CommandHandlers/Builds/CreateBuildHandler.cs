using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.Builds.Mappers;
using ReleaseBoard.Domain.Repositories;

namespace ReleaseBoard.Application.CommandHandlers.Builds
{
    /// <summary>
    /// Команда создания билда.
    /// </summary>
    public class CreateBuildHandler : CommandHandlerBase<CreateBuild>
    {
        private readonly IAggregateRepository aggregateRepository;
        private readonly IBuildLifeCycleStateMapper lifeCycleStateMapper;
        private readonly ILogger<CreateBuildHandler> logger;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="aggregateRepository"><see cref="IAggregateRepository"/>.</param>
        /// <param name="lifeCycleStateMapper"><see cref="IBuildLifeCycleStateMapper"/>.</param>
        /// <param name="logger">Logging.</param>
        public CreateBuildHandler(
            IAggregateRepository aggregateRepository,
            IBuildLifeCycleStateMapper lifeCycleStateMapper,
            ILogger<CreateBuildHandler> logger) : 
            base(logger)
        {
            this.aggregateRepository = aggregateRepository;
            this.lifeCycleStateMapper = lifeCycleStateMapper;
            this.logger = logger;
        }

        /// <inheritdoc />
        protected override async Task HandleCore(CreateBuild command)
        {
            logger.LogInformation($"Received command CreateBuild: {command}");

            LifeCycleState lifeCycleState = lifeCycleStateMapper.MapFromSuffixes(command.Suffixes);

            logger.LogInformation($"Suffixes were mapped to {lifeCycleState}. {command}.");

            Build aggregate = new (
                command.BuildDate,
                command.Number,
                command.ReleaseNumber,
                command.DistributionId,
                command.Location,
                command.SourceType,
                lifeCycleState,
                command.Suffixes);

            await aggregateRepository.Add(aggregate);

            logger.LogInformation($"Build aggregate Id:{aggregate.Id} has been created successfully. {command}");
        }
    }
}
