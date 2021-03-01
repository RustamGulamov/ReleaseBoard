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
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Distributions;
using ReleaseBoard.Domain.Distributions.Commands;
using ReleaseBoard.Domain.Distributions.Events;
using ReleaseBoard.Domain.Distributions.Events.BuildBindings;
using ReleaseBoard.Domain.Distributions.Events.LifeCycleStates;
using ReleaseBoard.Domain.Distributions.Events.ProjectBindings;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.Domain.Services;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.UnitTests.DataGenerators;
using ReleaseBoard.UnitTests.Domain.EventStores;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Distributions.Commands
{
    /// <summary>
    /// Тесты для обработки команды на создание дистрибутива.
    /// </summary>
    public class UpdateDistributionHandlerTests : DistributionTests
    {
        private readonly IRequestHandler<UpdateDistribution, Unit> updateDistributionHandler;
        private readonly EventStore eventStore;
        private readonly Guid distributionId = Guid.NewGuid();

        /// <summary>
        /// Конструктор.
        /// </summary>
        public UpdateDistributionHandlerTests()
        {
            eventStore = new EventStore(new MemoryChangesetRepository(), new Mock<IMediator>().Object);
            updateDistributionHandler = new UpdateDistributionHandler(
                new AggregateRepository(eventStore, new Mock<ILogger<AggregateRepository>>().Object),
                new CollectionsComparer(),
                new Mock<ILogger<UpdateDistributionHandler>>().Object
            );
            CreateDistributionInStream();
        }

        private UpdateDistribution RandomUpdateCommand => new()
        {
            Id = distributionId,
            Name = FakeGenerator.GetString(), 
            Owners = Owners.Skip(1).Append(FakeGenerator.GetUser()).ToArray(), 
            BuildBindings = FakeGenerator.BuildsBindings.Generate(10).ToArray(), 
            ProjectBindings = FakeGenerator.ProjectBindings.Generate(1).ToArray(),
            ActionUser = FakeGenerator.GetUser(),
            AvailableLifeCycles = new LifeCycleState[] { LifeCycleState.Build, LifeCycleState.Release, LifeCycleState.Certified },
            LifeCycleStateRules = new string[] { "some rules", "other rules" }
        };
        
        /// <summary>
        /// Тест на выполнение команды. Проходит если в стриме появился объект distribution.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task Handle_CommandValid_DistributionValid()
        {
            UpdateDistribution command = RandomUpdateCommand;

            await updateDistributionHandler.Handle(command, CancellationToken.None);

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
            var eventStream = await eventStore.LoadEventStream<Distribution>(distributionId);
            return new Distribution(eventStream.Events);
        }
        
        private void CreateDistributionInStream()
        {
            Metadata metadata = new Metadata() { UserId = FakeGenerator.GetString(), AggregateId = distributionId };
            
            eventStore.AppendToStream<Distribution>(distributionId,
                new List<Event>()
                {
                    new DistributionCreated(distributionId, DistributionName, Owners, new LifeCycleState[] { LifeCycleState.Build }, LifeCycleStateRules, metadata),
                    new DistributionAvailableLifeCyclesAdded(new LifeCycleState[] { LifeCycleState.Archival }, metadata),
                    new DistributionLifeCycleStateRulesAdded(new [] { "some rules" }, metadata),
                    new BuildBindingsAdded(new Dictionary<BuildsBinding, Guid[]>()
                    {
                        [FakeGenerator.GetBuildsBinding()] = new Guid[] { Guid.NewGuid(), Guid.NewGuid() },
                        [FakeGenerator.GetBuildsBinding()] = new Guid[] { }
                    }, metadata),
                    new ProjectBindingAdded(FakeGenerator.ProjectBindings.Generate(1)[0], metadata),
                    new ProjectBindingAdded(FakeGenerator.ProjectBindings.Generate(1)[0], metadata),
                    new ProjectBindingAdded(FakeGenerator.ProjectBindings.Generate(1)[0], metadata),
                }).GetAwaiter().GetResult();
        }
    }
}
