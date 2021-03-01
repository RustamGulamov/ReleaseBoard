using System;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.ReadModels
{
    /// <summary>
    /// Модель привязки проекта.
    /// </summary>
    public class ProjectBindingReadModel : IEquatable<ProjectBindingReadModel>
    {
        /// <summary>
        /// Проект.
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// Маска версии проекта.
        /// </summary>
        public string MaskProjectVersion { get; set; }

        /// <inheritdoc />
        public bool Equals(ProjectBindingReadModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(Project, other.Project) && MaskProjectVersion == other.MaskProjectVersion;
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

            return Equals((ProjectBindingReadModel)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Project, MaskProjectVersion);
    }
}
