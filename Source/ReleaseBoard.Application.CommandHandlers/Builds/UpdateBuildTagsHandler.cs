using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Commands;
using ReleaseBoard.Domain.Repositories;
using ReleaseBoard.Domain.Services;

namespace ReleaseBoard.Application.CommandHandlers.Builds
{
    /// <inheritdoc />
    public class UpdateBuildTagsHandler : CommandHandlerBase<UpdateBuildTags>
    {
        private readonly IAggregateRepository aggregateRepository;
        private readonly ILogger<UpdateBuildTagsHandler> logger;
        private readonly ICollectionsComparer collectionsComparer;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="aggregateRepository"><see cref="IAggregateRepository"/>.</param>
        /// <param name="logger">Logging.</param>
        /// <param name="collectionsComparer"><see cref="ICollectionsComparer"/>.</param>
        public UpdateBuildTagsHandler(
            IAggregateRepository aggregateRepository,
            ILogger<UpdateBuildTagsHandler> logger,
            ICollectionsComparer collectionsComparer) :
            base(logger)
        {
            this.aggregateRepository = aggregateRepository;
            this.logger = logger;
            this.collectionsComparer = collectionsComparer;
        }

        /// <inheritdoc />
        protected override async Task HandleCore(UpdateBuildTags command)
        {
            await aggregateRepository.UpdateById<Build>(
                command.BuildId,
                build =>
                {
                    UpdateTags(build, command);
                    return Task.CompletedTask;
                }
            );
        }

        private void UpdateTags(Build build, UpdateBuildTags command)
        {
            (IList<string> newTags, IList<string> tagsToDelete) =
                collectionsComparer
                    .Compare(build.Tags, command.Tags);

            if (!newTags.Any() && !tagsToDelete.Any())
            {
                return;
            }

            foreach (string tag in tagsToDelete)
            {
                build.RemoveTag(tag);
            }

            foreach (string newTag in newTags)
            {
                build.AddTag(newTag);
            }

            build.UpdateArtifactState(ArtifactState.Updated);
        }
    }
}
