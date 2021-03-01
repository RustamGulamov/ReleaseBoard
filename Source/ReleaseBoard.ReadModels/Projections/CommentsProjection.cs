using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Events;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.Metadata;

namespace ReleaseBoard.ReadModels.Projections
{
    /// <summary>
    /// Проекция для комментария.
    /// </summary>
    public class CommentsProjection : INotificationHandler<LifeCycleStateChanged>
    {
        private readonly IRepository<BuildChangeStateCommentReadModel> commentsRepository;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="commentsRepository"><see cref="IRepository{BuildChangeStateCommentReadModel}"/>.</param>
        public CommentsProjection(IRepository<BuildChangeStateCommentReadModel> commentsRepository)
        {
            this.commentsRepository = commentsRepository;
        }

        /// <inheritdoc />
        public async Task Handle(LifeCycleStateChanged @event, CancellationToken cancellationToken)
        {
            (LifeCycleState lifeCycleState, IMetadata metadata) = @event;

            if (metadata.TryGetValue(MetadataKeys.Comment, out string value))
            {
                await commentsRepository.Add(
                    new BuildChangeStateCommentReadModel
                    {
                        BuildId = metadata.AggregateId,
                        Comment = value,
                        NewState = lifeCycleState,
                        CreatedAt = metadata.Timestamp.DateTime,
                        UserId = metadata.UserId
                    }, cancellationToken);
            }
        }
    }
}
