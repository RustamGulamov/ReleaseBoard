using System;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Extensions;
using ReleaseBoard.Domain.ValueObjects.Exceptions;

namespace ReleaseBoard.Domain.ValueObjects
{
    /// <summary>
    /// Cвязка с проектом.
    /// </summary>
    public class ProjectBinding : ValueObject<ProjectBinding>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="maskProjectVersion">Маска для версии.</param>
        /// <param name="project">Проект.</param>
        public ProjectBinding(string maskProjectVersion, Project project)
        {
            maskProjectVersion.ThrowIsNullOrWhiteSpace(new CreateValueObjectException(nameof(ProjectBinding), "maskProjectVersion empty"));
            project.ThrowIfNull(new CreateValueObjectException(nameof(ProjectBinding), "Project not defined"));

            MaskProjectVersion = maskProjectVersion;
            Project = project;
        }

        /// <summary>
        /// Маска версии проекта.
        /// </summary>
        public string MaskProjectVersion { get; }

        /// <summary>
        /// Проект.
        /// </summary>
        public Project Project { get; }

        /// <inheritdoc />
        protected override bool EqualsCore(ProjectBinding other)
        {
            return Project.Equals(other.Project) && 
                MaskProjectVersion == other.MaskProjectVersion;
        }

        /// <inheritdoc />
        protected override int GetHashCodeCore()
        {
            return HashCode.Combine(Project, MaskProjectVersion);
        }
    }
}
