using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Common.Contracts.Extensions;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.ReadModels
{
    /// <summary>
    /// Модель дистрибутива для чтения(DTO).
    /// </summary>
    public class DistributionReadModel : Entity, IEquatable<DistributionReadModel>
    {
        /// <summary>
        /// Название дистрибутива.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Владельцы дистрибутива.
        /// </summary>
        public List<User> Owners { get; set; } = new();

        /// <summary>
        /// Список доступных состояний сборок у дистрибутива.
        /// </summary>
        public List<LifeCycleState> AvailableLifeCycles { get; set; } = new();

        /// <summary>
        /// Привязки к сборкам.
        /// </summary>
        public List<BuildBindingReadModel> BuildBindings { get; set; } = new();

        /// <summary>
        /// Привязки к проектам.
        /// </summary>
        public List<ProjectBindingReadModel> ProjectBindings { get; set; } = new();

        /// <inheritdoc />
        public bool Equals(DistributionReadModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name &&
                Owners.SequenceEqual(other.Owners) &&
                AvailableLifeCycles.SequenceEqual(other.AvailableLifeCycles) &&
                BuildBindings.SequenceEqual(other.BuildBindings) &&
                ProjectBindings.SequenceEqual(other.ProjectBindings);
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

            return Equals((DistributionReadModel)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => 
            HashCode
                .Combine(
                    Name, Id,
                    BuildBindings.GetSequenceHashCode(),
                    ProjectBindings.GetSequenceHashCode(),
                    AvailableLifeCycles.GetSequenceHashCode(),
                    Owners.GetSequenceHashCode());
    }
}
