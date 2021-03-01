using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.Extensions;
using MediatR;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.EventStore
{
    /// <summary>
    /// Хранилище событий для доступа к потокам событий.
    /// </summary>
    public class EventStore : IEventStore
    {
        private readonly IChangesetRepository changesetRepository;
        private readonly IMediator mediator;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="changesetRepository"><see cref="IChangesetRepository"/>.</param>
        /// <param name="mediator"><see cref="IMediator"/>.</param>
        public EventStore(
            IChangesetRepository changesetRepository,
            IMediator mediator)
        {
            this.changesetRepository = changesetRepository;
            this.mediator = mediator;
        }

        /// <inheritdoc />
        public Task<EventStream> LoadEventStream(string streamName) => 
            LoadEventStreamByStreamName(streamName, 0, int.MaxValue);

        /// <inheritdoc />
        public Task<EventStream> LoadEventStream<TAggregate>(Guid id) => 
            LoadEventStreamAfterVersion<TAggregate>(id, 0);

        /// <inheritdoc />
        public Task<EventStream> LoadEventStreamAfterVersion<TAggregate>(Guid id, long afterVersion)
        {
            string streamName = CreateStreamName<TAggregate>(id);
            return LoadEventStreamByStreamName(streamName, afterVersion, int.MaxValue);
        }

        /// <inheritdoc />
        public async Task AppendToStream<TAggregate>(Guid id, IReadOnlyCollection<Event> events, long expectedVersion = -1)
        {
            ThrowIfNotValid(expectedVersion);

            if (events.IsEmpty())
            {
                return;
            }

            await changesetRepository.Append(CreateStreamName<TAggregate>(id), events, expectedVersion);

            foreach (Event @event in events)
            {
                await mediator.Publish(@event);
            }
        }

        private async Task<EventStream> LoadEventStreamByStreamName(string streamName, long afterVersion, int limit)
        {
            ThrowIfNotValid(afterVersion);

            IEnumerable<EventsWithVersion> records = 
                await changesetRepository.Read(streamName, afterVersion, limit);

            return CreateEventStream(records);
        }

        private EventStream CreateEventStream(IEnumerable<EventsWithVersion> records) =>
            new EventStream
            {
                Version = records.Any() ? records.Select(x => x.Version).Max() : 0,
                Events  = records.SelectMany(x => x.Events).ToList()
            };

        private string CreateStreamName<T>(Guid id) => $"{typeof(T).Name}-{id}";

        private void ThrowIfNotValid(long version)
        {
            if (version < -1)
            {
                throw new ArgumentException($"Not valid stream version: {version}");
            }
        }
    }
}
