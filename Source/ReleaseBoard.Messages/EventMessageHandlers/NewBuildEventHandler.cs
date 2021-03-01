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
    /// Обработчик событий создания сборки.
    /// </summary>
    public class NewBuildEventHandler : EventHandlerBase<NewBuildEvent>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="mediator"><see cref="IMediator"/>.</param>
        /// <param name="mapper"><see cref="IMapper"/>.</param>
        /// <param name="validator">Валидатор.</param>
        /// <param name="logger">Логгер.</param>
        public NewBuildEventHandler(
            IMediator mediator, 
            IMapper mapper,
            IValidator<NewBuildEvent> validator, 
            ILogger<NewBuildEventHandler> logger):
            base(mediator, mapper, validator, logger)
        {
        }
        
        /// <inheritdoc />
        protected override async Task HandleCore(NewBuildEvent eventMessage)
        {
            await SendCommandIfValidMessage<CreateBuild>(eventMessage);
        }
    }
}
