using System;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Common.Infrastructure.Rabbit.Extensions;
using ReleaseBoard.Common.Infrastructure.Rabbit.Interfaces;
using ReleaseBoard.Common.Infrastructure.Rabbit.Interfaces.Handlers;
using ReleaseBoard.Common.Infrastructure.Rabbit.Options;
using RawRabbit;

namespace ReleaseBoard.Infrastructure.BuildSync
{
    /// <summary>
    /// Потребитель событий от BuildSync.
    /// </summary>
    public class BuildSyncEventsConsumer : IEventMessageConsumer
    {
        private readonly IBusClient busClient;
        private readonly ReleaseBoardRabbitSettings releaseBoardRabbitSettings;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="releaseBoardRabbitSettings">Конфигурация клиента RabbitMq.</param>
        /// <param name="busClient"><see cref="RabbitConsumerExtensions"/>.</param>
        public BuildSyncEventsConsumer(ReleaseBoardRabbitSettings releaseBoardRabbitSettings, IBusClient busClient)
        {
            this.busClient = busClient ?? throw new ArgumentNullException(nameof(busClient));
            this.releaseBoardRabbitSettings = releaseBoardRabbitSettings ?? throw new ArgumentNullException(nameof(releaseBoardRabbitSettings));
        }

        /// <inheritdoc />
        public void RegisterEventHandler<TRequest>(IEventMessageHandler<TRequest> messageHandler)
            where TRequest : IEventMessage
        {
            busClient.SubscribeOnEvents(messageHandler, new ConsumeEventsOptions
            {
                EventExchange = releaseBoardRabbitSettings.ReleaseBoardBuildSyncExchange,
                EventQueue = releaseBoardRabbitSettings.BuildSyncEventsQueue
            });
        }
    }
}
