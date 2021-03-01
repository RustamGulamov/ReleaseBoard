using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Server;
using ReleaseBoard.Common.Contracts.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Domain.SignalREvents.Job;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.Infrastructure.BuildSync;

namespace ReleaseBoard.Infrastructure.Hangfire
{
    /// <inheritdoc />
    public class BackgroundJobWithNotificationService : IBackgroundJobWithNotificationService
    {
        private readonly IMediator mediator;
        private readonly ILogger<BackgroundJobWithNotificationService> logger;
        private readonly IBuildSyncService buildSyncService;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="mediator">Медиатр.</param>
        /// <param name="logger">Логгер.</param>
        /// <param name="buildSyncService"><see cref="IBuildSyncService"/>.</param>
        public BackgroundJobWithNotificationService(
            IMediator mediator,
            ILogger<BackgroundJobWithNotificationService> logger,
            IBuildSyncService buildSyncService)
        {
            this.mediator = mediator;
            this.logger = logger;
            this.buildSyncService = buildSyncService;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BuildDto>> ParseBuilds(BuildsBinding[] buildBindings, string userId, PerformContext context)
        {
            string jobId = context.BackgroundJob.Id;
            
            logger.LogInformation($"Try parse builds for {buildBindings.Select(x => $"{x}")}. UserId: ${userId}. JobId: ${jobId}");
            await mediator.Publish(CreateBuildParseNotification(jobId, userId, 0));

            IEnumerable<BuildDto> buildsDto = await buildSyncService.ParseBuilds(buildBindings);
            
            await mediator.Publish(CreateBuildParseNotification(jobId, userId, 100));
            logger.LogInformation($"Parsed {buildsDto.Count()} builds successfully!. UserId: ${userId}. JobId: ${jobId}");

            return buildsDto;
        }

        private JobSignalREvent CreateBuildParseNotification(string jobId, string actionUserId, int percent) => 
            new(JobEventType.BuildParse, new JobEventPayload(jobId, percent), actionUserId);
    }
}
