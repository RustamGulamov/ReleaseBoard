using System;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.ReadModels
{
    /// <summary>
    /// Модель комментария при изменении состояния сборки.
    /// </summary>
    public class BuildChangeStateCommentReadModel : Entity, IEquatable<BuildChangeStateCommentReadModel>
    {
        /// <summary>
        /// Идентификатор сборки.
        /// </summary>
        public Guid BuildId { get; set; }

        /// <summary>
        /// Комментария изменении, если есть.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Пользователь, который оставил комментарий.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Новое состояние билда.
        /// </summary>
        public LifeCycleState NewState { get; set; }

        /// <summary>
        /// Дата создания.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <inheritdoc />
        public bool Equals(BuildChangeStateCommentReadModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return 
                BuildId.Equals(other.BuildId) && 
                Comment == other.Comment &&
                Equals(UserId, other.UserId) && 
                CreatedAt == other.CreatedAt && 
                NewState == other.NewState;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((BuildChangeStateCommentReadModel)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => 
            HashCode.Combine(BuildId, Comment, UserId, CreatedAt, NewState);
    }
}
