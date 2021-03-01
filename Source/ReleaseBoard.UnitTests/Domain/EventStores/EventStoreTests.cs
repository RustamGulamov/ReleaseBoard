using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Distributions;
using ReleaseBoard.Domain.Distributions.Events;
using ReleaseBoard.Domain.Distributions.Events.BuildBindings;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.EventStore.Exceptions;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.UnitTests.DataGenerators;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.EventStores
{
    /// <summary>
    /// Тесты для <see cref="IEventStore"/>.
    /// </summary>
    public class EventStoreTests
    {
        private readonly Guid id = Guid.NewGuid();
        private readonly IEventStore eventStore;
        private readonly IMetadata metadata;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public EventStoreTests()
        {
            eventStore = new EventStore(new MemoryChangesetRepository(), new Mock<IMediator>().Object);
            metadata = new Metadata() { AggregateId = id };
        }

        private DistributionCreated DistributionCreated => new DistributionCreated(
            id,
            "Name",
            FakeGenerator.Users.Generate(1).ToArray(),
            new LifeCycleState[] { LifeCycleState.Build, LifeCycleState.Release },
            new string[] { LifeCycleState.Release.ToString() },
            metadata
        );

        private BuildBindingsAdded BuildBindingEvent => new(new Dictionary<BuildsBinding, Guid[]>(), metadata);

        /// <summary>
        /// Создать новый поток события для дистрибутива.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task CreateNewStream_CreateDistributionEvent_CreateDistributionEventAddedToStorage()
        {
            var eventStream = CreateEventStream(-1, DistributionCreated);

            await eventStore.AppendToStream<Distribution>(id, eventStream.Events.ToList());

            var result = await eventStore.LoadEventStream<Distribution>(id);
            Assert.Equal(1, result.Version);
            Assert.Equal(((DistributionCreated)eventStream.Events.First()).Id, ((DistributionCreated)result.Events.First()).Id);
        }

        /// <summary>
        /// Добавить поток события для дистрибутива.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task AppendToStream_DistributionCreatedAndBuildBindingEvent_EventsAddedToStorage()
        {
            await eventStore.AppendToStream<Distribution>(id, new List<Event>() { DistributionCreated });
            var eventStream = CreateEventStream(1, BuildBindingEvent);

            await eventStore.AppendToStream<Distribution>(id, eventStream.Events.ToList(), eventStream.Version);

            var result = await eventStore.LoadEventStream<Distribution>(id);
            Assert.Equal(2, result.Version);
            Assert.Equal(new List<Type>() { typeof(DistributionCreated), typeof(BuildBindingsAdded) }, result.Events.Select(x => x.GetType()));
        }

        /// <summary>
        /// Тест проверяет выброс исключение <see cref="ChangesetConcurrencyException"/>, если номер версии указана неправильно.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task AppendToStream_WrongVersion_ThrowsAsync()
        {
            await eventStore.AppendToStream<Distribution>(id, new List<Event>() { DistributionCreated });
            var eventStream = CreateEventStream(2, BuildBindingEvent);

            await Assert.ThrowsAsync<ChangesetConcurrencyException>(() =>
                eventStore.AppendToStream<Distribution>(id, eventStream.Events.ToList(), eventStream.Version));
        }

        private EventStream CreateEventStream(long version, params Event[] events) =>
            new()
            {
                Version = version,
                Events = new List<Event>(events)
            };
    }
}
