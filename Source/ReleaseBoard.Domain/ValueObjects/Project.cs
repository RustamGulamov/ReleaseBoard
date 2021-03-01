using System;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Extensions;
using ReleaseBoard.Domain.ValueObjects.Exceptions;

namespace ReleaseBoard.Domain.ValueObjects
{
    /// <summary>
    /// Проект.
    /// </summary>
    public class Project : ValueObject<Project>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="externalId">Внешний идентификатор.</param>
        /// <param name="shortName">Краткое имя проекта.</param>
        /// <param name="name">Имя проекта.</param>
        public Project(Guid externalId, string shortName, string name)
        {
            if (externalId == Guid.Empty)
            {
                throw new CreateValueObjectException($"Project {name} externalId is not defined");
            }
            name.ThrowIsNullOrWhiteSpace(new CreateValueObjectException(nameof(Project),"name is not defined"));
            shortName.ThrowIsNullOrWhiteSpace(new CreateValueObjectException(nameof(Project), "name is not defined"));

            ExternalId = externalId;
            ShortName = shortName;
            Name = name;
        }

        /// <summary>
        /// Внешний идентификатор проекта.
        /// </summary>
        public Guid ExternalId { get; }

        /// <summary>
        /// Краткое имя проекта.
        /// </summary>
        public string ShortName { get; }

        /// <summary>
        /// Полное имя проекта.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name}, {ShortName}";
        }

        /// <inheritdoc />
        protected override bool EqualsCore(Project other)
        {
            return 
                ExternalId == other.ExternalId && 
                ShortName == other.ShortName && 
                Name == other.Name;
        }

        /// <inheritdoc />
        protected override int GetHashCodeCore()
        {
            return HashCode.Combine(ExternalId, ShortName, Name);
        }
    }
}
