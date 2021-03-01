using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Force.DeepCloner;
using MediatR;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.Distributions.Events;
using ReleaseBoard.Domain.Distributions.Events.BuildBindings;
using ReleaseBoard.Domain.Distributions.Events.LifeCycleStates;
using ReleaseBoard.Domain.Distributions.Events.Owners;
using ReleaseBoard.Domain.Distributions.Events.ProjectBindings;
using ReleaseBoard.Domain.Distributions.Exceptions;
using ReleaseBoard.Domain.SignalREvents.Server;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.ReadModels.Projections.Notification;

namespace ReleaseBoard.ReadModels.Projections
{
    /// <summary>
    /// Проекция для события дистрибутива.
    /// </summary>
    public class DistributionEventsProjection :
        INotificationHandler<DistributionCreated>,
        INotificationHandler<DistributionNameUpdated>,
        INotificationHandler<DistributionOwnersAdded>,
        INotificationHandler<DistributionOwnersRemoved>,
        INotificationHandler<DistributionAvailableLifeCyclesAdded>,
        INotificationHandler<DistributionAvailableLifeCyclesRemoved>,
        INotificationHandler<BuildBindingsAdded>,
        INotificationHandler<BuildBindingsRemoved>,
        INotificationHandler<ProjectBindingAdded>,
        INotificationHandler<ProjectBindingRemoved>
    {
        private readonly IRepository<DistributionReadModel> distributionRepository;
        private readonly IReadOnlyRepository<BuildMatchPatternReadModel> buildMatchPatternRepository;
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="distributionRepository"><see cref="IRepository{DistributionReadModel}"/>.</param>
        /// <param name="buildMatchPatternRepository"><see cref="IReadOnlyRepository{BuildMatchPatternReadModel}"/>.</param>
        /// <param name="mediator"><see cref="IMediator"/>.</param>
        /// <param name="mapper"><see cref="IMapper"/>.</param>
        public DistributionEventsProjection(
            IRepository<DistributionReadModel> distributionRepository, 
            IReadOnlyRepository<BuildMatchPatternReadModel> buildMatchPatternRepository,
            IMediator mediator,
            IMapper mapper)
        {
            this.distributionRepository = distributionRepository;
            this.buildMatchPatternRepository = buildMatchPatternRepository;
            this.mediator = mediator;
            this.mapper = mapper;
        }
        
        /// <inheritdoc />
        public async Task Handle(DistributionCreated @event, CancellationToken cancellationToken)
        {
            var newDistribution = mapper.Map<DistributionReadModel>(@event);

            if (await distributionRepository.Any(x => x.Name == @event.Name, cancellationToken))
            {
                throw new CreateDistributionException($"Distribution {@event.Name} already exist.");
            }

            await distributionRepository.Add(newDistribution, cancellationToken);

            await SendNotification(ServerEventType.DistributionCreated, null, newDistribution);
        }

        /// <inheritdoc />
        public async Task Handle(DistributionNameUpdated @event, CancellationToken cancellationToken)
        {
            await Update(@event, distribution => distribution.Name = @event.Name, cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(DistributionOwnersAdded @event, CancellationToken cancellationToken)
        {
            await Update(@event, distribution => distribution.Owners.AddRange(@event.Owners), cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(DistributionOwnersRemoved @event, CancellationToken cancellationToken)
        {
            await Update(@event, distribution => distribution.Owners.RemoveAll(@event.Owners.Contains), cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(DistributionAvailableLifeCyclesAdded @event, CancellationToken cancellationToken)
        {
            await Update(
                @event, 
                distribution => distribution.AvailableLifeCycles.AddRange(@event.AvailableLifeCycles), 
                cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(DistributionAvailableLifeCyclesRemoved @event, CancellationToken cancellationToken)
        {
            await Update(
                @event,
                distribution => distribution.AvailableLifeCycles.RemoveAll(@event.AvailableLifeCycles.Contains),
                cancellationToken);
        }
        
        /// <inheritdoc />
        public async Task Handle(BuildBindingsAdded @event, CancellationToken cancellationToken)
        {
            var buildBindings = await MapToReadModels(@event.BindingToBuilds.Keys);

            await Update(@event, distribution => distribution.BuildBindings.AddRange(buildBindings), cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(BuildBindingsRemoved @event, CancellationToken cancellationToken)
        {
            var buildBindings = await MapToReadModels(@event.BindingsToBuilds.Keys);

            await Update(@event, distribution => distribution.BuildBindings.RemoveAll(x => buildBindings.Contains(x)), cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(ProjectBindingAdded @event, CancellationToken cancellationToken)
        {
            var readModel = mapper.Map<ProjectBindingReadModel>(@event.ProjectBinding);

            await Update(@event, distribution => distribution.ProjectBindings.Add(readModel), cancellationToken);
        }

        /// <inheritdoc />
        public async Task Handle(ProjectBindingRemoved @event, CancellationToken cancellationToken)
        {
            var readModel = mapper.Map<ProjectBindingReadModel>(@event.ProjectBinding);

            await Update(@event, distribution => distribution.ProjectBindings.Remove(readModel), cancellationToken);
        }

        private async Task Update(Event @event, Action<DistributionReadModel> update, CancellationToken cancellationToken)
        {
            Guid distributionId = @event.Metadata.AggregateId;

            DistributionReadModel distribution = await distributionRepository.Query(b => b.Id == distributionId, cancellationToken);
            if (distribution == null)
            {
                throw new UpdateDistributionException($"Distribution {distributionId} not found.");
            }

            DistributionReadModel oldDistribution = distribution.DeepClone();

            update(distribution);
            
            await distributionRepository.Update(x => x.Id == distributionId, distribution, cancellationToken);

            await SendNotification(ServerEventType.DistributionUpdated, oldDistribution, distribution);
        }
        
        private async Task SendNotification(string type, DistributionReadModel oldDistribution, DistributionReadModel newDistribution)
        {
            await mediator.Publish(new SendDiffEntitiesNotifications.Notification(type, oldDistribution, newDistribution, newDistribution?.Owners));
        }

        private async Task<IEnumerable<BuildBindingReadModel>> MapToReadModels(IEnumerable<BuildsBinding> buildsBindings)
        {
            var readModels = new List<BuildBindingReadModel>();

            foreach (var buildBinding in buildsBindings)
            {
                var buildBindingReadModel = mapper.Map<BuildBindingReadModel>(buildBinding);
                buildBindingReadModel.Pattern = await buildMatchPatternRepository.Query(x => x.Regexp == buildBinding.Pattern.Regexp);
                readModels.Add(buildBindingReadModel);
            }

            return readModels;
        }
    }
}
