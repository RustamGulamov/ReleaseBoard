using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ReleaseBoard.Application.CommandHandlers.Distributions;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Distributions;
using ReleaseBoard.Domain.Distributions.Commands;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.UnitTests.DataGenerators;
using ReleaseBoard.UnitTests.Domain.EventStores;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Distributions.Commands
{
    /// <summary>
    /// Тесты для обработки команды на создание дистрибутива.
    /// </summary>
    public class CreateDistributionHandlerTests
    {
        private readonly IRequestHandler<CreateDistribution, Unit> createDistributionHandler;
        private readonly EventStore eventStore;
        private readonly MemoryChangesetRepository memoryChangesetRepository = new MemoryChangesetRepository();

        /// <summary>
        /// Конструктор.
        /// </summary>
        public CreateDistributionHandlerTests()
        {
            eventStore = new EventStore(memoryChangesetRepository, new Mock<IMediator>().Object);
            createDistributionHandler = new CreateDistributionHandler(
                new AggregateRepository(eventStore, new Mock<ILogger<AggregateRepository>>().Object), 
                new Mock<ILogger<CreateDistributionHandler>>().Object
            );
        }

        private CreateDistribution RandomCreateCommand => new CreateDistribution()
        {
            Name = FakeGenerator.GetString(), 
            Owners = new List<User> { FakeGenerator.GetUser() }, 
            ActionUser = FakeGenerator.GetUser(),
            AvailableLifeCycles = new LifeCycleState[] { LifeCycleState.Build, LifeCycleState.ReleaseCandidate },
            LifeCycleStateRules = new []{ FakeGenerator.GetString() },
            BuildBindings = FakeGenerator.BuildsBindings.Generate(2).ToArray(), 
            ProjectBindings = FakeGenerator.ProjectBindings.Generate(1).ToArray(),
        };
        
        /// <summary>
        /// Тест на выполнение команды. Проходит если в стриме появился объект distribution.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task Handle_CommandValid_DistributionValidAndBackgroundTaskStarted()
        {
            CreateDistribution command = RandomCreateCommand;

            await createDistributionHandler.Handle(command, CancellationToken.None);

            Distribution distribution = await RestoreDistribution();
            Assert.Equal(command.Name, distribution.Name);
            Assert.Equal(command.Owners, distribution.Owners);
            Assert.Equal(command.BuildBindings, distribution.BindingToBuilds.Keys);
            Assert.Equal(command.ProjectBindings, distribution.ProjectBindings);
            Assert.Equal(command.LifeCycleStateRules, distribution.LifeCycleStateRules);
            Assert.Equal(command.AvailableLifeCycles, distribution.AvailableLifeCycles);
        }

        private async Task<Distribution> RestoreDistribution()
        {
            var streamName = memoryChangesetRepository.EventsStorage.First().Name;
            var eventStream = await eventStore.LoadEventStream(streamName);
            return new Distribution(eventStream.Events);
        }
    }
}
