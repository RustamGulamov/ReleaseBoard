using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ReleaseBoard.Application.CommandHandlers.Builds;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.UnitTests.Domain.EventStores;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Builds.Commands
{
    /// <summary>
    /// Тесты для класса <see cref="MarkAsTrackedBuildHandler"/>.
    /// </summary>
    public class MarkAsTrackedBuildHandlerTests : BuildTests
    {
        private readonly IRequestHandler<MarkAsTrackedBuild, Unit> markAsTrackedBuildHandler;
        private readonly EventStore eventStore;
        private readonly Build build;
        private readonly MarkAsTrackedBuild fakeCommand;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public MarkAsTrackedBuildHandlerTests()
        {
            eventStore = new EventStore(new MemoryChangesetRepository(), new Mock<IMediator>().Object);
            markAsTrackedBuildHandler = new MarkAsTrackedBuildHandler(
                new AggregateRepository(eventStore, new Mock<ILogger<AggregateRepository>>().Object),
                new Mock<ILogger<MarkAsTrackedBuildHandler>>().Object
            );

            build = CreateBuild();

            fakeCommand = new MarkAsTrackedBuild()
            {
                BuildId = build.Id, 
                DistributionId = DistributionId, 
                MarkDate = DateTime.Now.AddDays(10)
            };
        }

        /// <summary>
        /// Тест проверяет изменение статуса билда на ReturnToTracked, если отправить команду MarkAsTrackedBuild.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_BuildUnTracked_ReturnToTracked()
        {
            build.MarkAsUnTracked(DateTime.Now);
            await eventStore.AppendToStream<Build>(build.Id, build.GetUncommitedChanges());

            await markAsTrackedBuildHandler.Handle(fakeCommand, CancellationToken.None);

            var editedBuild = await RestoreBuild();
            Assert.Equal(fakeCommand.DistributionId, editedBuild.DistributionId);
            Assert.Equal(ArtifactState.ReturnToTracked, editedBuild.ArtifactState);
            Assert.False(editedBuild.IsUnTracked);
        }

        /// <summary>
        /// Тест проверяет не изменился ли статус билда, если отправить команду MarkAsTrackedBuild и статус билда Tracked.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_BuildTracked_BuildHasNotChanged()
        {
            await eventStore.AppendToStream<Build>(build.Id, build.GetUncommitedChanges());

            await markAsTrackedBuildHandler.Handle(fakeCommand, CancellationToken.None);

            var editedBuild = await RestoreBuild();
            Assert.Equal(DistributionId, editedBuild.DistributionId);
            Assert.Equal(ArtifactState.Created, editedBuild.ArtifactState);
            Assert.False(editedBuild.IsUnTracked);
        }

        private async Task<Build> RestoreBuild()
        {
            var eventStream = await eventStore.LoadEventStream<Build>(build.Id);
            return new Build(eventStream.Events);
        }
    }
}
