using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.Repositories;

namespace ReleaseBoard.Application.CommandHandlers.Builds
{
    /// <summary>
    /// Базовый хэндлер комманд обнволения сборок.
    /// </summary>
    /// <typeparam name="T">Тип комманды.</typeparam>
    public abstract class UpdateBuildBaseHandler<T> : CommandHandlerBase<T> where T : UpdateBuildCommandBase
    {
        /// <summary>
        /// Реплзиторий моделей.
        /// </summary>
        protected readonly IAggregateRepository AggregateRepository;
        
        /// <summary>
        /// Логгер.
        /// </summary>
        protected readonly ILogger<CommandHandlerBase<T>> Logger;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="aggregateRepository"><see cref="IAggregateRepository"/>.</param>
        /// <param name="logger">Logging.</param>
        protected UpdateBuildBaseHandler(
            IAggregateRepository aggregateRepository,
            ILogger<CommandHandlerBase<T>> logger) :
            base(logger)
        {
            AggregateRepository = aggregateRepository;
            Logger = logger;
        }

        /// <inheritdoc />
        protected override async Task HandleCore(T command) =>
            await AggregateRepository.UpdateById<Build>(command.BuildId, build => UpdateBuild(build, command));

        /// <summary>
        /// Метод обновления сборки.
        /// </summary>
        /// <param name="build">Сборка.</param>
        /// <param name="command">Комманда.</param>
        /// <returns>Задачу обновления.</returns>
        protected abstract Task UpdateBuild(Build build, T command);
    }
}
