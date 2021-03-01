using System;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Common.Infrastructure.Rabbit.Interfaces;
using ReleaseBoard.Common.Infrastructure.Rabbit.Interfaces.Handlers;

namespace ReleaseBoard.Messages
{
    /// <summary>
    /// Регистратор обработчиков сообщений.
    /// </summary>
    public class HandlersRegistrator : IHandlersRegistrator
    {
        private readonly IEventMessageConsumer buildSyncerEventsConsumer;
        private readonly IEventMessageHandler<IEventMessage> eventMessagesDispatcher;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="buildSyncerEventsConsumer">Потребитель событий от buildSyncer сервисов.</param>
        /// <param name="eventMessagesDispatcher">Диспетчер событий из очереди.</param>
        public HandlersRegistrator(
            IEventMessageConsumer buildSyncerEventsConsumer,
            IEventMessageHandler<IEventMessage> eventMessagesDispatcher)
        {
            this.buildSyncerEventsConsumer = buildSyncerEventsConsumer;
            this.eventMessagesDispatcher = eventMessagesDispatcher;
        }

        /// <summary>
        /// Регистрирует все обработчики.
        /// </summary>
        public void RegisterAll()
        {
            buildSyncerEventsConsumer.RegisterEventHandler(eventMessagesDispatcher);
        }
    }
}
