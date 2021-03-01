using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using ReleaseBoard.Common.Contracts.BuildSync.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Builds.Commands;

namespace ReleaseBoard.Messages.EventMessageHandlers
{
    /// <summary>
    /// Обработчик события удаления сборки.
    /// </summary>
    public class DeleteBuildEventHandler : EventHandlerBase<DeleteBuildEvent>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="mediator"><see cref="IMediator"/>.</param>
        /// <param name="mapper"><see cref="IMapper"/>.</param>
        /// <param name="validator">Валидатор.</param>
        /// <param name="logger">Логгер.</param>
        public DeleteBuildEventHandler(
            IMediator mediator,
            IMapper mapper,
            IValidator<DeleteBuildEvent> validator,
            ILogger<DeleteBuildEventHandler> logger):
            base(mediator, mapper, validator, logger)
        {
        }

        /// <inheritdoc />
        protected override async Task HandleCore(DeleteBuildEvent eventMessage)
        {
            await SendCommandIfValidMessage<DeleteBuild>(eventMessage);
        }
    }
}
