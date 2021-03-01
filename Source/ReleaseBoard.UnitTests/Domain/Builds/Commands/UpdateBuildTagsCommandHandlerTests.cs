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
using ReleaseBoard.Domain.Services;
using ReleaseBoard.UnitTests.Domain.EventStores;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Builds.Commands
{
    /// <summary>
    /// Тесты для <see cref="UpdateBuildTags"/>.
    /// </summary>
    public class UpdateBuildTagsCommandHandlerTests : BuildTests
    {
        private readonly IRequestHandler<UpdateBuildTags, Unit> updateBuildTagsHandler;
        private readonly Build build;
        private readonly EventStore eventStore;

        /// <summary>
        /// Конструктор.
        /// </summary>
        public UpdateBuildTagsCommandHandlerTests()
        {
            eventStore = new EventStore(new MemoryChangesetRepository(), new Mock<IMediator>().Object);

            updateBuildTagsHandler = new UpdateBuildTagsHandler(
                new AggregateRepository(eventStore, new Mock<ILogger<AggregateRepository>>().Object),
                new Mock<ILogger<UpdateBuildTagsHandler>>().Object,
                new CollectionsComparer()
            );
            build = CreateBuild();
            eventStore.AppendToStream<Build>(build.Id, build.GetUncommitedChanges()).GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// Тест обработчика команды обновления тегов билда, если добавялется тег.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_AddTag_BuildTagAdded()
        {
            var command = new UpdateBuildTags()
            {
                BuildId = build.Id,
                Tags = new[] { "tag1" }
            };

            await updateBuildTagsHandler.Handle(command, CancellationToken.None);

            var editedBuild = await RestoreBuild();
            Assert.Equal(command.Tags, editedBuild.Tags);
            Assert.Equal(ArtifactState.Updated, editedBuild.ArtifactState);
        }

        /// <summary>
        /// Тест обработчика команды обновления тегов билда, если удаляется тег.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_RemoveTag_BuildTagRemoved()
        {
            build.AddTag("new_Tag");
            build.AddTag("new_Tag_2");
            await eventStore.AppendToStream<Build>(build.Id, build.GetUncommitedChanges());
            var command = new UpdateBuildTags()
            {
                BuildId = build.Id,
                Tags = Array.Empty<string>()
            };

            await updateBuildTagsHandler.Handle(command, CancellationToken.None);

            var editedBuild = await RestoreBuild();
            Assert.Equal(command.Tags, editedBuild.Tags);
            Assert.Equal(ArtifactState.Updated, editedBuild.ArtifactState);
        }

        /// <summary>
        /// Тест обработчика команды обновления тегов билда, если удаляется один тег.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task Handler_RemoveTag_RemovedOneTag()
        {
            build.AddTag("new_Tag");
            build.AddTag("new_Tag_2");
            await eventStore.AppendToStream<Build>(build.Id, build.GetUncommitedChanges());
            var command = new UpdateBuildTags()
            {
                BuildId = build.Id,
                Tags = new []{ "new_Tag" }
            };

            await updateBuildTagsHandler.Handle(command, CancellationToken.None);

            var editedBuild = await RestoreBuild();
            Assert.Equal(command.Tags, editedBuild.Tags);
            Assert.Equal(ArtifactState.Updated, editedBuild.ArtifactState);
        }

        private async Task<Build> RestoreBuild()
        {
            var eventStream = await eventStore.LoadEventStream<Build>(build.Id);
            return new Build(eventStream.Events);
        }
    }
}
