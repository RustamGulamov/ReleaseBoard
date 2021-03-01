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
    public class DeleteBuildHandler : CommandHandlerBase<DeleteBuild>
    {
        private readonly IAggregateRepository aggregateRepository;
        private readonly ILogger<DeleteBuildHandler> logger;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="aggregateRepository"><see cref="IAggregateRepository"/>.</param>
        /// <param name="logger">Logging.</param>
        public DeleteBuildHandler(
            IAggregateRepository aggregateRepository, 
            ILogger<DeleteBuildHandler> logger) : 
            base(logger)
        {
            this.aggregateRepository = aggregateRepository;
            this.logger = logger;
        }

        /// <inheritdoc />
        protected override async Task HandleCore(DeleteBuild command)
        {
            logger.LogInformation($"Received command DeleteBuild: BuildId: {command.BuildId}.");

            await aggregateRepository.UpdateById<Build>(command.BuildId,
                build =>
                {
                    build.MarkAsUnTracked(command.DeleteDate);
                    return Task.CompletedTask;
                });

            logger.LogInformation($"Build aggregate Id: { command.BuildId } has been deleted successfully.");
        }
    }
}
