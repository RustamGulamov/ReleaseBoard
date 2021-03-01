using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.EventStore.Exceptions;

namespace ReleaseBoard.Application.CommandHandlers
{
    /// <summary>
    /// Базовый обработчик.
    /// </summary>
    /// <typeparam name="T">Тип комманды.</typeparam>
    public abstract class CommandHandlerBase<T> : AsyncRequestHandler<T>
        where T : ICommand
    {
        private readonly ILogger<CommandHandlerBase<T>> logger;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        protected CommandHandlerBase(ILogger<CommandHandlerBase<T>> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Метод обработки команды.
        /// </summary>
        /// <param name="command">Комманда.</param>
        /// <param name="cancellationToken">a.</param>
        /// <returns>Таск.</returns>
        protected override async Task Handle(T command, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    await HandleCore(command);
                    return;
                }
                catch (ChangesetConcurrencyException ex)
                {
                    logger.LogError(ex);
                }
            }
        }

        /// <summary>
        /// Метод реализующий основную логику.
        /// </summary>
        /// <param name="command">Команда.</param>
        /// <returns><see cref="Task"/>.</returns>
        protected abstract Task HandleCore(T command);
    }
}
