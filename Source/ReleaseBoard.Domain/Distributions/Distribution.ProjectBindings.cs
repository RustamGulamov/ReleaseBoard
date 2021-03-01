using System;
using ReleaseBoard.Domain.Distributions.Events.ProjectBindings;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions
{
    /// <summary>
    /// Дистрибутив. Методы изменения состояния привязки к проектам.
    /// </summary>
    public partial class Distribution
    {
        /// <summary>
        /// Обновляет привязку.
        /// </summary>
        /// <param name="projectBinding">Привязка к проекту.</param>
        /// <param name="actionUserId">Идентификатор пользователя, кто добавил привязки.</param>
        public void AddProjectBinding(ProjectBinding projectBinding, string actionUserId)
        {
            if (ProjectBindings.Contains(projectBinding))
            {
                return;
            }

            Apply(new ProjectBindingAdded(projectBinding, CreateMetadata(userId: actionUserId)));
        }

        /// <summary>
        /// Удаляет привязку.
        /// </summary>
        /// <param name="projectBinding">Привязка к проекту.</param>
        /// <param name="actionUserId">Идентификатор пользователя, кто добавил привязки.</param>
        public void RemoveProjectBinding(ProjectBinding projectBinding, string actionUserId)
        {
            if (!ProjectBindings.Contains(projectBinding))
            {
                return;
            }

            Apply(new ProjectBindingRemoved(projectBinding, CreateMetadata(userId: actionUserId)));
        }

        private void When(ProjectBindingAdded @event)
        {
            ProjectBindings.Add(@event.ProjectBinding);
        }

        private void When(ProjectBindingRemoved @event)
        {
            ProjectBindings.Remove(@event.ProjectBinding);
        }
    }
}
