using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Domain.SignalREvents.Abstractions;
using ReleaseBoard.Domain.SignalREvents.Job;
using ReleaseBoard.Web.ApiModels;
using ReleaseBoard.Web.Enums;
using ReleaseBoard.Web.Providers;

namespace ReleaseBoard.Web.SignalR
{
    /// <summary>
    /// Хаб нотификации.
    /// </summary>
    public class NotificationHub : Hub<ISignalRClient>, INotificationHub
    {
        private readonly ILogger<NotificationHub> logger;
        private readonly IAuthorizedUserProvider authorizedUserProvider;
        private readonly IJobStorageService jobStorageService;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        /// <param name="authorizedUserProvider"><see cref="IAuthorizedUserProvider"/>.</param>
        /// <param name="jobStorageService">Сервис для работы <see cref="JobStorage"/>.</param>
        public NotificationHub(
            ILogger<NotificationHub> logger, 
            IAuthorizedUserProvider authorizedUserProvider, 
            IJobStorageService jobStorageService)
        {
            this.logger = logger;
            this.authorizedUserProvider = authorizedUserProvider;
            this.jobStorageService = jobStorageService;
        }

        /// <summary>
        /// Публиковать события для выполнимых задач.
        /// </summary>
        /// <param name="limit">Лимит.</param>
        /// <returns><see cref="Task"/>.</returns>
        [Authorize]
        public async Task PublishJobEventsForProcessingJobs(int limit)
        {
            AuthorizedUser user = authorizedUserProvider.Get();

            foreach ((string jobId, string eventType) in jobStorageService.GetAllActiveJobsForUser(user.Sid, limit))
            {
                await PublishToCurrentUser(new JobSignalREvent(eventType, new JobEventPayload(jobId, 0), user.Sid));
            }
        }

        /// <inheritdoc />
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            
            AuthorizedUser currentUser = authorizedUserProvider.Get();
            await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(currentUser));
            
            logger.LogInformation($"{currentUser} connected to notifications.");
        }
        
        /// <inheritdoc />
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);

            AuthorizedUser currentUser = authorizedUserProvider.Get();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(currentUser));
            
            logger.LogInformation($"{currentUser} disconnected from notifications.");
        }

        private async Task PublishToCurrentUser(SignalREvent @event)
        {
            await Clients.Client(Context.ConnectionId).ReceiveEvent(@event);
        }

        private static string GetGroupName(AuthorizedUser currentUser) => 
            currentUser.IsAdmin 
                ? Role.Admin.ToString() 
                : currentUser.Sid;
    }
}
