using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ReleaseBoard.Application.CommandHandlers.Builds.ChangeState;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.Builds.Mappers;
using ReleaseBoard.Domain.Builds.StateChangeChecker;
using ReleaseBoard.Domain.Core.Exceptions;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.UnitTests.DataGenerators;
using ReleaseBoard.UnitTests.Domain.EventStores;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Builds.Commands
{
    /// <summary>
    /// Тесты для комманды <see cref="ChangeBuildState"/>.
    /// </summary>
    public class ChangeBuildStateHandlerTests : BuildTests
    {
        private readonly IRequestHandler<ChangeBuildState, Unit> changeBuildStateHandler;
        private readonly Mock<IBuildLifeCycleStateMapper> buildLifeCycleStateMapperMock = new Mock<IBuildLifeCycleStateMapper>();
        private readonly Mock<IBuildStateChangeChecker> buildStateChangeCheckerMock = new Mock<IBuildStateChangeChecker>();
        private readonly Mock<IBuildSyncService> buildSyncServiceMock = new Mock<IBuildSyncService>();
        private readonly EventStore eventStore;
        private readonly Build build;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public ChangeBuildStateHandlerTests()
        {
            eventStore = new EventStore(new MemoryChangesetRepository(), new Mock<IMediator>().Object);

            changeBuildStateHandler = new ChangeBuildStateHandler(
                new AggregateRepository(eventStore, new Mock<ILogger<AggregateRepository>>().Object),
                new Mock<ILogger<ChangeBuildStateHandler>>().Object,
                buildStateChangeCheckerMock.Object,
                buildLifeCycleStateMapperMock.Object,
                buildSyncServiceMock.Object
            );

            build = CreateBuild();
            
            eventStore.AppendToStream<Build>(build.Id, build.GetUncommitedChanges()).GetAwaiter().GetResult();
        }

        private ChangeBuildState FakeCommand => new ChangeBuildState
        {
            BuildId = build.Id,
            Comment = "Some_comment",
            NewState = LifeCycleState.Release,
            User = FakeGenerator.GetUser()
        };

        /// <summary>
        /// Тест обработчика команды изменения состоянии билда.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_CanChange_CreatedEventLifeCycleStateChanged()
        {
            SetupInit(true, true);
            await eventStore.AppendToStream<Build>(build.Id, build.GetUncommitedChanges());

            await changeBuildStateHandler.Handle(FakeCommand, CancellationToken.None);

            var editedBuild = await RestoreBuild();
            Assert.Equal(FakeCommand.NewState, editedBuild.LifeCycleState);
            Assert.Equal(ArtifactState.Updated, editedBuild.ArtifactState);
            Assert.Equal(FakeCommand.BuildId, editedBuild.Id);
            VerifyChangeSuffixMethod();
        }

        /// <summary>
        /// Тест обработчика команды изменения состоянии билда, если не соответствует правилам.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_CannotChange_Throws()
        {
            SetupInit(false, true);

            await Assert.ThrowsAsync<ValidationException>(
                async () => await changeBuildStateHandler.Handle(FakeCommand, CancellationToken.None)
            );
        }

        /// <summary>
        /// Тест обработчика команды изменения состоянии билда, если buildsync возвращал IsSuccessful = false.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_BuildSyncReturnFail_Throws()
        {
            SetupInit(true, false);

            await Assert.ThrowsAsync<ValidationException>(
                async () => await changeBuildStateHandler.Handle(FakeCommand, CancellationToken.None));
        }

        private void SetupInit(bool canChangeBuildState, bool isSuccessfulChangeResponse)
        {
            buildStateChangeCheckerMock.Setup(x => x.Check(It.IsAny<Build>(), It.IsAny<LifeCycleState>()))
                .Returns(Task.FromResult(new StateChangeCheckResult() { CanChange = canChangeBuildState }));

            var response =
                isSuccessfulChangeResponse
                    ? (IResponseMessage)new ChangeSuffixesResponse { IsSuccessful = true }
                    : new ErrorResponseMessage();
            
            buildSyncServiceMock
                .Setup(x => x.ChangeSuffix(
                    It.IsAny<string>(),
                    It.IsAny<BuildSourceType>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>()))
                .ReturnsAsync(response);

            buildLifeCycleStateMapperMock
                .Setup(x => x.MapToSuffix(LifeCycleState.Release))
                .Returns("R");

            buildLifeCycleStateMapperMock
                .Setup(x => x.MapToSuffix(LifeCycleState.ReleaseCandidate))
                .Returns("RC");
        }

        private void VerifyChangeSuffixMethod()
        {
            buildSyncServiceMock.Verify(
                x => x.ChangeSuffix(
                    It.IsAny<string>(),
                    It.IsAny<BuildSourceType>(),
                    It.IsAny<List<string>>(),
                    It.IsAny<List<string>>()
                ),
                Times.Once());
        }

        private async Task<Build> RestoreBuild()
        {
            var eventStream = await eventStore.LoadEventStream<Build>(build.Id);
            return new Build(eventStream.Events);
        }
    }
}
