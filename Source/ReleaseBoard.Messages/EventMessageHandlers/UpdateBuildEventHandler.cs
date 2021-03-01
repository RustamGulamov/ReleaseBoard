using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using ReleaseBoard.Common.Contracts.BuildSync.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Messages.Validators;

namespace ReleaseBoard.Messages.EventMessageHandlers
{
    /// <summary>
    /// Потребитель события обновления сборки.
    /// </summary>
    public class UpdateBuildEventHandler : EventHandlerBase<UpdateBuildEvent>
    {
        private readonly ILogger<UpdateBuildEventHandler> logger;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="mediator"><see cref="IMediator"/>.</param>
        /// <param name="mapper"><see cref="IMapper"/>.</param>
        /// <param name="validator">Валидатор.</param>
        /// <param name="logger">Логгер.</param>
        public UpdateBuildEventHandler(
            IMediator mediator,
            IMapper mapper,
            IValidator<UpdateBuildEvent> validator,
            ILogger<UpdateBuildEventHandler> logger): 
            base(mediator, mapper, validator, logger)
        {
            this.logger = logger;
        }
        
        /// <inheritdoc />
        protected override async Task HandleCore(UpdateBuildEvent eventMessage)
        {
            await SendCommandIfValidMessage<UpdateBuild>(eventMessage);
        }
    }
}
