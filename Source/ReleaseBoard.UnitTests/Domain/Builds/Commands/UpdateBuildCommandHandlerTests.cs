using System;
using System.Collections.Generic;
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
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.UnitTests.Domain.EventStores;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Builds.Commands
{
    /// <summary>
    /// Тесты для <see cref="UpdateBuildHandler"/>.
    /// </summary>
    public class UpdateBuildCommandHandlerTests : BuildTests
    {
        private readonly IRequestHandler<UpdateBuild, Unit> updateBuildHandler;
        private readonly EventStore eventStore;
        private readonly Mock<IBuildLifeCycleStateMapper> buildLifeCycleStateMapperMock = new Mock<IBuildLifeCycleStateMapper>();
        private readonly Build build;
        private readonly UpdateBuild fakeCommand;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public UpdateBuildCommandHandlerTests()
        {
            eventStore = new EventStore(new MemoryChangesetRepository(), new Mock<IMediator>().Object);
            updateBuildHandler = new UpdateBuildHandler(
                new AggregateRepository(eventStore, new Mock<ILogger<AggregateRepository>>().Object),
                new Mock<ILogger<UpdateBuildHandler>>().Object,
                buildLifeCycleStateMapperMock.Object
            );

            build = CreateBuild();

            fakeCommand = new UpdateBuild { BuildId = build.Id, ChangeDate = DateTime.Now };
        }

        /// <summary>
        /// Тест обработчика команды обновления билда, если изменения соответствуют правилам.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_UpdateBuild_BuildUpdated()
        {
            SetupInit(LifeCycleState.Release);
            fakeCommand.Location = Location + "_R";
            fakeCommand.Number = new VersionNumber("1.0.0.1");
            fakeCommand.ReleaseNumber = new VersionNumber("1.0.0");
            fakeCommand.Suffixes = new List<string>() { "R" };
            await eventStore.AppendToStream<Build>(build.Id, build.GetUncommitedChanges());

            await updateBuildHandler.Handle(fakeCommand, CancellationToken.None);

            var updatedBuild = await RestoreBuild();
            Assert.Equal(fakeCommand.Location, updatedBuild.Location);
            Assert.Equal(fakeCommand.Suffixes, updatedBuild.Suffixes);
            Assert.Equal(fakeCommand.Number, updatedBuild.Number);
            Assert.Equal(fakeCommand.ReleaseNumber, updatedBuild.ReleaseNumber);
            Assert.Equal(fakeCommand.BuildId, updatedBuild.Id);
            Assert.Equal(LifeCycleState.Release, updatedBuild.LifeCycleState);
        }

        /// <summary>
        /// Тест обработчика команды обновления билда, если изменения соответствуют правилам.
        /// Тест проверяет работу обработчика, если уже установлено статус билда LifeCycleState = Release.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_UpdateBuild_LifeCycleStateAlreadyRelease_BuildUpdatedAndLifeCycleStateHasNotChanged()
        {
            SetupInit(LifeCycleState.Release);
            fakeCommand.Location = Location + "_R";
            fakeCommand.Number = new VersionNumber("1.0.0.1");
            fakeCommand.ReleaseNumber = new VersionNumber("1.0.0");
            fakeCommand.Suffixes = new List<string>() { "R" };
            build.UpdateLifeCycleState(LifeCycleState.Release, DateTime.Now);
            await eventStore.AppendToStream<Build>(build.Id, build.GetUncommitedChanges());

            await updateBuildHandler.Handle(fakeCommand, CancellationToken.None);

            var updatedBuild = await RestoreBuild();
            Assert.Equal(fakeCommand.Location, updatedBuild.Location);
            Assert.Equal(fakeCommand.Suffixes, updatedBuild.Suffixes);
            Assert.Equal(fakeCommand.Number, updatedBuild.Number);
            Assert.Equal(fakeCommand.ReleaseNumber, updatedBuild.ReleaseNumber);
            Assert.Equal(fakeCommand.BuildId, updatedBuild.Id);
            Assert.Equal(LifeCycleState.Release, updatedBuild.LifeCycleState);
        }

        private void SetupInit(LifeCycleState buildLifeCycleStateMapperValue)
        {
            buildLifeCycleStateMapperMock
                .Setup(x => x.MapFromSuffixes(It.IsAny<string[]>()))
                .Returns(buildLifeCycleStateMapperValue);
        }

        private async Task<Build> RestoreBuild()
        {
            var eventStream = await eventStore.LoadEventStream<Build>(build.Id);
            return new Build(eventStream.Events);
        }
    }
}
