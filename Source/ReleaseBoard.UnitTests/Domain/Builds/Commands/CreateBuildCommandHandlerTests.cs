using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ReleaseBoard.Application.CommandHandlers.Builds;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.Builds.Mappers;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.UnitTests.Domain.EventStores;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Builds.Commands
{
    /// <summary>
    /// Тесты для <see cref="CreateBuildHandler"/>.
    /// </summary>
    public class CreateBuildCommandHandlerTests : BuildTests
    {
        private readonly IRequestHandler<CreateBuild, Unit> createBuildHandler;
        private readonly Mock<IBuildLifeCycleStateMapper> buildLifeCycleStateMapperMock = new Mock<IBuildLifeCycleStateMapper>();
        private readonly EventStore eventStore;
        private readonly MemoryChangesetRepository memoryEventHistoryRepository = new MemoryChangesetRepository();

        /// <summary>
        /// Конструктор.
        /// </summary>
        public CreateBuildCommandHandlerTests()
        {
            eventStore = new EventStore(memoryEventHistoryRepository, new Mock<IMediator>().Object);
            createBuildHandler = new CreateBuildHandler(
                new AggregateRepository(eventStore, new Mock<ILogger<AggregateRepository>>().Object),
                buildLifeCycleStateMapperMock.Object, 
                new Mock<ILogger<CreateBuildHandler>>().Object
                );
        }
        
        /// <summary>
        /// Тест обработчика команды создания билда c состоянием Build.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_CreateBuild_BuildCreated()
        {
            SetupBuildLifeCycleStateMapper(LifeCycleState.Build);
            CreateBuild fakeCommand = CreateBuildCommand();

            await createBuildHandler.Handle(fakeCommand, CancellationToken.None);

            var createdBuild = await RestoreBuild();
            Assert.Equal(fakeCommand.Location, createdBuild.Location);
            Assert.Equal(fakeCommand.BuildDate, createdBuild.CreatedAt);
            Assert.Equal(fakeCommand.Number, createdBuild.Number);
            Assert.Equal(fakeCommand.DistributionId, createdBuild.DistributionId);
            Assert.Equal(LifeCycleState.Build, createdBuild.LifeCycleState);
        }

        /// <summary>
        /// Тест обработчика команды создания билда, c состоянием Release.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_CreateReleaseBuild_BuildCreated()
        {
            SetupBuildLifeCycleStateMapper(LifeCycleState.Release);
            CreateBuild fakeCommand = CreateBuildCommand();
            fakeCommand.Suffixes = new[] { "R" };
            fakeCommand.Location += "_R";

            await createBuildHandler.Handle(fakeCommand, CancellationToken.None);
            
            var createdBuild = await RestoreBuild();
            Assert.Equal(fakeCommand.Location, createdBuild.Location);
            Assert.Equal(fakeCommand.BuildDate, createdBuild.CreatedAt);
            Assert.Equal(fakeCommand.Number, createdBuild.Number);
            Assert.Equal(fakeCommand.DistributionId, createdBuild.DistributionId);
            Assert.Equal(fakeCommand.Suffixes, createdBuild.Suffixes);
            Assert.Equal(LifeCycleState.Release, createdBuild.LifeCycleState);
        }

        private CreateBuild CreateBuildCommand()
        {
            return new()
            {
                BuildDate = BuildDate,
                DistributionId = DistributionId,
                ReleaseNumber = ReleaseNumber,
                Number = Number,
                Location = Location
            };
        }

        private void SetupBuildLifeCycleStateMapper(LifeCycleState returnState)
        {
            buildLifeCycleStateMapperMock
                .Setup(x => x.MapFromSuffixes(It.IsAny<string[]>()))
                .Returns(returnState);
        }

        private async Task<Build> RestoreBuild()
        {
            var streamName = memoryEventHistoryRepository.EventsStorage.First().Name;
            var eventStream = await eventStore.LoadEventStream(streamName);
            return new Build(eventStream.Events);
        }
    }
}
