using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Force.DeepCloner;
using MediatR;
using ReleaseBoard.Domain.Builds.Events;
using ReleaseBoard.Domain.Builds.Exceptions;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.SignalREvents.Server;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.ReadModels.Projections.Notification;

namespace ReleaseBoard.ReadModels.Projections
{
    /// <summary>
    /// Проекция для события билда.
    /// </summary>
    public class BuildEventsProjection : 
        INotificationHandler<BuildCreated>,
        INotificationHandler<BuildUpdated>,
        INotificationHandler<BuildMarkAsUnTracked>,
        INotificationHandler<BuildMarkAsTracked>,
        INotificationHandler<LifeCycleStateChanged>,
        INotificationHandler<BuildTagAdded>,
        INotificationHandler<BuildTagRemoved>
    {
        private readonly IRepository<BuildReadModel> buildRepository;
        private readonly IReadOnlyRepository<DistributionReadModel> distributionsRepository;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="buildRepository"><see cref="IRepository{BuildReadModel}"/>.</param>
        /// <param name="distributionsRepository"><see cref="IReadOnlyRepository{DistributionReadModel}"/>.</param>
        /// <param name="mapper"><see cref="IMapper"/>.</param>
        /// <param name="mediator"><see cref="IMediator"/>.</param>
        public BuildEventsProjection(
            IRepository<BuildReadModel> buildRepository, 
            IReadOnlyRepository<DistributionReadModel> distributionsRepository,
            IMapper mapper,
            IMediator mediator)
        {
            this.buildRepository = buildRepository;
            this.distributionsRepository = distributionsRepository;
            this.mapper = mapper;
            this.mediator = mediator;
        }

        /// <inheritdoc />
        public async Task Handle(BuildCreated @event, CancellationToken cancellationToken)
        {
            var newBuild = mapper.Map<BuildReadModel>(@event);

            if (await buildRepository.Any(x => x.Id == @event.Id, cancellationToken))
            {
                throw new CreateBuildException($"Build {@event.Id} already exist.");
            }

            await buildRepository.Add(newBuild, cancellationToken);

            await SendNotification(ServerEventType.BuildCreated, null, newBuild);
        }

        /// <inheritdoc />
        public async Task Handle(BuildUpdated @event, CancellationToken cancellationToken)
        {
            await Update(@event,
                build =>
                {
                    build.Location = @event.Location;
                    build.Number = @event.Number.ToString();
                    build.ReleaseNumber = @event.ReleaseNumber.ToString();
                    build.Suffixes = @event.Suffixes.ToList();
                },
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(BuildMarkAsUnTracked @event, CancellationToken cancellationToken)
        {
            await Update(@event,
                build =>
                {
                    build.IsUnTracked = true;
                    build.DistributionId = Guid.Empty;
                },
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(BuildMarkAsTracked @event, CancellationToken cancellationToken)
        {
            await Update(@event,
                build =>
                {
                    build.IsUnTracked = false;
                    build.DistributionId = @event.DistributionId;
                },
                cancellationToken);
        }


        /// <inheritdoc />
        public async Task Handle(LifeCycleStateChanged @event, CancellationToken cancellationToken)
        {
            await Update(@event, build => build.LifeCycleState = @event.LifeCycleState, cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(BuildTagAdded @event, CancellationToken cancellationToken)
        {
            await Update(@event, build => build.Tags.Add(@event.Tag), cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(BuildTagRemoved @event, CancellationToken cancellationToken)
        {
            await Update(@event, build => build.Tags.Remove(@event.Tag), cancellationToken);
        }

        private async Task Update(Event @event, Action<BuildReadModel> update, CancellationToken cancellationToken)
        {
            Guid buildId = @event.Metadata.AggregateId;

            BuildReadModel build = await buildRepository.Query(b => b.Id == buildId, cancellationToken);
            if (build == null)
            {
                throw new UpdateBuildException($"Build {buildId} not found.");
            }

            BuildReadModel oldBuild = build.DeepClone();

            update(build);

            await buildRepository.Update(x => x.Id == buildId, build, cancellationToken);

            await SendNotification(ServerEventType.BuildUpdated, oldBuild, build);
        }
        
        private async Task SendNotification(string type, BuildReadModel oldBuild, BuildReadModel newBuild)
        {
            Guid distributionId = newBuild.IsUnTracked ? oldBuild.DistributionId : newBuild.DistributionId;
            List<User> users = await distributionsRepository.ProjectOne(x => x.Id == distributionId, x => x.Owners);
            
            await mediator.Publish(new SendDiffEntitiesNotifications.Notification(type, oldBuild, newBuild, users));
        }
    }
}
