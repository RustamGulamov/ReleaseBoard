using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Common.Contracts.Extensions;
using ReleaseBoard.Common.Infrastructure.Rabbit.Interfaces.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Messages.EventMessageHandlers
{
    /// <summary>
    /// Базовый обрвботчик события.
    /// </summary>
    /// <typeparam name="T">тип события.</typeparam>
    public abstract class EventHandlerBase<T> : IEventMessageHandler<T> where T : IEventMessage
    {
        private readonly IMapper mapper;
        private readonly IValidator<T> validator;
        private readonly ILogger<EventHandlerBase<T>> logger;
        private readonly IMediator mediator;
        
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="mediator"><see cref="IMediator"/>.</param>
        /// <param name="mapper"><see cref="IMapper"/>.</param>
        /// <param name="validator">Валидатор.</param>
        /// <param name="logger">Логгер.</param>
        protected EventHandlerBase(
            IMediator mediator,
            IMapper mapper,
            IValidator<T> validator,
            ILogger<EventHandlerBase<T>> logger)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task Handle(T eventMessage)
        {
            if (eventMessage == null)
            {
                throw new ArgumentNullException($"{typeof(T).Name} is null.");
            }
            
            logger.LogInformation($"Received message {nameof(T)}. {eventMessage}");
            
            await HandleCore(eventMessage);
        }

        /// <summary>
        /// Обработать сообщение.
        /// </summary>
        /// <param name="eventMessage">сообщение.</param>
        /// <returns><see cref="Task"/>.</returns>
        protected abstract Task HandleCore(T eventMessage);

        /// <summary>
        /// Валидировать и отправить команду.
        /// </summary>
        /// <param name="eventMessage">Сообщение.</param>
        /// <param name="onFailureValidation">Action, если валидация не прошла.</param>
        /// <param name="onValid">Action, после отпавки команды.</param>
        /// <returns><see cref="Task"/>.</returns>
        protected virtual async Task SendCommandIfValidMessage<TCommand>(T eventMessage, Func<ValidationResult, Task> onFailureValidation = null, Action<T> onValid = null) 
            where TCommand : ICommand
        {
            ValidationResult validationResult = await validator.ValidateAsync(eventMessage);
            if (!validationResult.IsValid)
            {
                logger.LogError(
                    $"{typeof(T).Name} message is not valid. {eventMessage}. " +
                    $"Errors: {validationResult.Errors?.Select(x => x.ErrorMessage).JoinWith(";")}");

                if (onFailureValidation != null)
                {
                    await onFailureValidation.Invoke(validationResult);
                }
                
                return;
            }
            
            onValid?.Invoke(eventMessage);

            await SendCommand<TCommand>(eventMessage);
        }
        
        /// <summary>
        /// Отправить команду.
        /// </summary>
        /// <param name="eventMessage">Сообщение.</param>
        /// <typeparam name="TCommand">Тип команды.</typeparam>
        /// <returns><see cref="Task"/>.</returns>
        protected async Task SendCommand<TCommand>(T eventMessage)
        {
            TCommand command = mapper.Map<TCommand>(eventMessage);
            await mediator.Send(command);
        }
    }
}
