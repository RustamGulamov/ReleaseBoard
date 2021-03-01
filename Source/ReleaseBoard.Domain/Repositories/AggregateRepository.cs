using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.Extensions;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Exceptions;
using ReleaseBoard.Domain.EventStore;

namespace ReleaseBoard.Domain.Repositories
{
    /// <inheritdoc />
    public class AggregateRepository : IAggregateRepository
    {
        private readonly IEventStore eventStore;
        private readonly ILogger<AggregateRepository> logger;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="eventStore"><see cref="IEventStore"/>.</param>
        /// <param name="logger">Логгер.</param>
        public AggregateRepository(IEventStore eventStore, ILogger<AggregateRepository> logger)
        {
            this.eventStore = eventStore;
            this.logger = logger;
        }

        /// <inheritdoc />
        public async Task Add<TAggregate>(TAggregate aggregate)
            where TAggregate : Aggregate
        {
            await Append(aggregate, -1);
        }

        /// <inheritdoc />
        public async Task<TAggregate> LoadById<TAggregate>(Guid id)
            where TAggregate : Aggregate
        {
            EventStream stream = await LoadEventStream<TAggregate>(id);

            return CreateAggregateByEvents<TAggregate>(stream.Events);
        }

        /// <inheritdoc />
        public async Task UpdateById<TAggregate>(Guid id, Func<TAggregate, Task> action)
            where TAggregate : Aggregate
        {
            EventStream stream = await LoadEventStream<TAggregate>(id);

            TAggregate aggregate = CreateAggregateByEvents<TAggregate>(stream.Events);

            await action(aggregate);

            await Append(aggregate, stream.Version);
        }

        private async Task<EventStream> LoadEventStream<TAggregate>(Guid id)
        {
            EventStream stream = await eventStore.LoadEventStream<TAggregate>(id);
            if (stream.Events.IsEmpty())
            {
                throw new DomainException($"Aggregate {typeof(TAggregate).Name} {id} not found for update.");
            }

            logger.LogInformation($"Loaded the {typeof(TAggregate).Name} aggregate by id: {id}");

            return stream;
        }

        private TAggregate CreateAggregateByEvents<TAggregate>(IReadOnlyList<Event> events)
            where TAggregate : Aggregate
        {
            logger.LogInformation($"Try to create the {typeof(TAggregate).Name} aggregate instance.");
            return (TAggregate)Activator.CreateInstance(typeof(TAggregate), events);
        }

        private async Task Append<TAggregate>(TAggregate aggregate, long version)
            where TAggregate : Aggregate
        {
            await eventStore.AppendToStream<TAggregate>(aggregate.Id, aggregate.GetUncommitedChanges(), version);

            logger.LogInformation($"Append the {typeof(TAggregate).Name} aggregate by id: {aggregate.Id} and version {version}.");

            aggregate.Commit();
        }
    }
}
