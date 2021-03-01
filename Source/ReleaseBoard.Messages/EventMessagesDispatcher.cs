using System;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Common.Contracts.BuildSync.Events;
using ReleaseBoard.Common.Infrastructure.Rabbit.Extensions;
using ReleaseBoard.Common.Infrastructure.Rabbit.Interfaces.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ReleaseBoard.Messages
{
    /// <summary>
    /// Диспетчер сообщений.
    /// </summary>
    public class EventMessagesDispatcher : IEventMessageHandler<IEventMessage>
    {
        private readonly ILogger<EventMessagesDispatcher> logger;
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/>.</param>
        /// <param name="logger">Логгер.</param>
        public EventMessagesDispatcher(IServiceProvider serviceProvider, ILogger<EventMessagesDispatcher> logger)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public Task Handle(IEventMessage request)
        {
            logger.LogDebug($"Got message of type {request.GetType()}");

            switch (request)
            {
                case NewBuildEvent r:
                    return ResolveHandlerAndCallAsync(r);
                case UpdateBuildEvent r:
                    return ResolveHandlerAndCallAsync(r);
                case DeleteBuildEvent r:
                    return ResolveHandlerAndCallAsync(r);
                default:
                    throw new InvalidOperationException($"No handlers found for request {request.GetType()}");
            }
        }

        private async Task ResolveHandlerAndCallAsync<T>(T requestMessage)
            where T : IEventMessage
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider
                    .GetService(typeof(IEventMessageHandler<T>)) as IEventMessageHandler<T>;

                await service.TryHandleEvent(requestMessage);
            }
        }
    }
}
