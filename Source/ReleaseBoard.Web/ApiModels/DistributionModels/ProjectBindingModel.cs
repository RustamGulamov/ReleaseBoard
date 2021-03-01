using System;
using ReleaseBoard.Domain.Distributions;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Web.ApiModels.DistributionModels
{
    /// <summary>
    /// Модель представляющий <see cref="ProjectBinding"/> для <see cref="Distribution"/>.
    /// </summary>
    public class ProjectBindingModel : IEquatable<ProjectBindingModel>
    {
        /// <summary>
        /// Внешний идентификатор проекта.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Маска версии проекта.
        /// </summary>
        public string MaskProjectVersion { get; set; }

        /// <inheritdoc />
        public bool Equals(ProjectBindingModel other)
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
                ProjectId.Equals(other.ProjectId) && 
                string.Equals(MaskProjectVersion, other.MaskProjectVersion, StringComparison.OrdinalIgnoreCase);
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

            return Equals((ProjectBindingModel)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return 
                    (ProjectId.GetHashCode() * 397) ^ 
                    (MaskProjectVersion != null ? MaskProjectVersion.GetHashCode() : 0);
            }
        }
    }
}
