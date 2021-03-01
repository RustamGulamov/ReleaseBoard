using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using ReleaseBoard.Domain.SignalREvents.Abstractions;
using ReleaseBoard.Domain.SignalREvents.Job;
using ReleaseBoard.Domain.SignalREvents.Server;
using ReleaseBoard.Web.Enums;

namespace ReleaseBoard.Web.SignalR
{
    /// <summary>
    /// Обработчик события<see cref="SignalREvent"/>.
    /// </summary>
    public class SignalREventHandler : 
        INotificationHandler<ServerSignalREvent>, 
        INotificationHandler<JobSignalREvent>
    {
        private readonly IHubContext<NotificationHub, ISignalRClient> hubContext;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="hubContext">Контекс хаб нотификации.</param>
        public SignalREventHandler(IHubContext<NotificationHub, ISignalRClient> hubContext) => 
            this.hubContext = hubContext;

        /// <inheritdoc />
        public Task Handle(ServerSignalREvent @event, CancellationToken cancellationToken) =>
            SendSignalREvent(@event, @event.RecipientsUserIds);

        /// <inheritdoc />
        public Task Handle(JobSignalREvent @event, CancellationToken cancellationToken) => 
            SendSignalREvent(@event, @event.UserId);

        private async Task SendSignalREvent(SignalREvent @event, params string[] recipientsUserIds)
        {
            IEnumerable<string> recipientsGroups = recipientsUserIds.Append(Role.Admin.ToString());
            await hubContext.Clients.Groups(recipientsGroups).ReceiveEvent(@event);
        }
    }
}
