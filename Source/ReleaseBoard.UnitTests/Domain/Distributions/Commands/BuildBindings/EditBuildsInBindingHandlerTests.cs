using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ReleaseBoard.Application.CommandHandlers.Distributions.BuildsBindings;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Distributions;
using ReleaseBoard.Domain.Distributions.Commands.BuildBindings;
using ReleaseBoard.Domain.Distributions.Events;
using ReleaseBoard.Domain.Distributions.Events.BuildBindings;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.UnitTests.DataGenerators;
using ReleaseBoard.UnitTests.Domain.EventStores;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Distributions.Commands.BuildBindings
{
    /// <summary>
    /// Тесты для класса <see cref="EditBuildsInBindingHandler"/>.
    /// </summary>
    public class EditBuildsInBindingHandlerTests : DistributionTests
    {
        private static readonly BuildMatchPattern pdcPattern = new(@"^(?<major>\d+\.\d+\.\d+)\\(?<number>\d+(\.\d+){1,2}_\(\d+\.\d+\))[\w]*$");
        private static readonly BuildMatchPattern artifactoryPattern = new(@"^(?<number>(?<major>(\d+\.){1,2}\d+)((.\d+){0,2}))[\w]*$");
        private readonly IRequestHandler<EditBuildsInBinding, Unit> editBuildsInBindingHandler;
        private readonly EventStore eventStore;
        private readonly Guid distributionId = Guid.NewGuid();

        private readonly BuildsBinding pkiClientBuildsBinding = new("ViPNet_Pki_Client", pdcPattern, BuildSourceType.Pdc);
        private readonly BuildsBinding coordinatorBuildsBinding = new("ViPNet_Coordinator_IG\\IG10", pdcPattern, BuildSourceType.Pdc);
        private readonly BuildsBinding artifactoryBuildsBinding = new("test-builds", artifactoryPattern, BuildSourceType.Artifactory);

        private readonly Guid[] pkiBuildIds = { Guid.NewGuid(), Guid.NewGuid() };
        private readonly Guid[] artifactoryBuildIds = { Guid.NewGuid(), Guid.NewGuid() };

        /// <summary>
        /// Конструктор.
        /// </summary>
        public EditBuildsInBindingHandlerTests()
        {
            eventStore = new EventStore(new MemoryChangesetRepository(), new Mock<IMediator>().Object);
            editBuildsInBindingHandler = new EditBuildsInBindingHandler(
                new AggregateRepository(eventStore, new Mock<ILogger<AggregateRepository>>().Object),
                new Mock<ILogger<EditBuildsInBindingHandler>>().Object
            );
            CreateDistributionInStream();
        }

        private EditBuildsInBinding RandomEditBuildsInBinding => new()
        {
            DistributionId = distributionId,
            BuildsToAdd = new Dictionary<Guid, string>()
            {
                [Guid.NewGuid()] = "ViPNet_Pki_Client\\1.0.0\\1.0_(0.449)",
                [Guid.NewGuid()] = "ViPNet_Pki_Client\\1.0.0",
                [Guid.NewGuid()] = "ViPNet_Pki_Client",
                [Guid.NewGuid()] = string.Empty,
                [Guid.NewGuid()] = null,
                [Guid.NewGuid()] = "test-builds/0.1.0_RC",
                [Guid.NewGuid()] = "ViPNet_Coordinator_IG\\IG10\\1.2.1\\1.2_(1.449)",
            },
            BuildIdsToRemove =
                pkiBuildIds.Take(1)
                    .Append(Guid.NewGuid()).ToArray() // Doesn't exist.
        };


        /// <summary>
        /// Тест на выполнение команды. Проходит подходщие сборки добавлются в привязке.
        /// </summary>
        /// <returns>Task.</returns>
        [Fact]
        public async Task Handle_CommandValid_MatchedBuildsAddedInBinding()
        {
            EditBuildsInBinding command = RandomEditBuildsInBinding;

            await editBuildsInBindingHandler.Handle(command, CancellationToken.None);

            Distribution distribution = await RestoreDistribution();
            var keys = command.BuildsToAdd.Keys.ToList();
            Assert.Equal(pkiBuildIds.Skip(1).Append(keys[0]), distribution.BindingToBuilds[pkiClientBuildsBinding]);
            Assert.Equal(artifactoryBuildIds.Union(new Guid[]{ keys[5] }), distribution.BindingToBuilds[artifactoryBuildsBinding]);
            Assert.Equal(new Guid[] { keys[6] }, distribution.BindingToBuilds[coordinatorBuildsBinding]);
        }

        private async Task<Distribution> RestoreDistribution()
        {
            var eventStream = await eventStore.LoadEventStream<Distribution>(distributionId);
            return new Distribution(eventStream.Events);
        }
        
        private void CreateDistributionInStream()
        {
            var user = FakeGenerator.GetUser();
            Metadata metadata = new Metadata() { AggregateId = distributionId, UserId = FakeGenerator.GetUser().Sid };
            eventStore.AppendToStream<Distribution>(distributionId,
                new List<Event>()
                {
                    new DistributionCreated(distributionId, DistributionName, Owners, AvailableLifeCycles, LifeCycleStateRules, metadata),
                    new BuildBindingsAdded(new Dictionary<BuildsBinding, Guid[]>()
                    {
                        [pkiClientBuildsBinding] = pkiBuildIds,
                    }, metadata),
                    new BuildBindingsAdded(new Dictionary<BuildsBinding, Guid[]>()
                    {
                        [artifactoryBuildsBinding] = artifactoryBuildIds,
                        [coordinatorBuildsBinding] = new Guid[] {}
                    }, metadata),
                }).GetAwaiter().GetResult();
        }
    }
}
