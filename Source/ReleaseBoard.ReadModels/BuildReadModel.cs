using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Common.Contracts.Extensions;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.ReadModels
{
    /// <summary>
    /// Модель билда для чтения(DTO).
    /// </summary>
    public class BuildReadModel : Entity, IEquatable<BuildReadModel>
    {
        /// <summary>
        /// Идентификатор связанного дистрибутива сборки.
        /// </summary>
        public Guid DistributionId { get; set; }

        /// <summary>
        /// <see cref="LifeCycleState"/>.
        /// </summary>
        public LifeCycleState LifeCycleState { get; set; }

        /// <summary>
        /// Тип хранилища.
        /// </summary>
        public BuildSourceType SourceType { get; set; }

        /// <summary>
        /// Теги сборки.
        /// </summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>
        /// Суффиксы сборки.
        /// </summary>
        public List<string> Suffixes { get; set; } = new();

        /// <summary>
        /// Номер сборки, например 4.2.2.36190.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Номер версии релиза, например 4.2.2.
        /// </summary>
        public string ReleaseNumber { get; set; }

        /// <summary>
        /// Относительный путь в хранилище сборок.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Дата сборки.
        /// </summary>
        public DateTime BuildDate { get; set; }

        /// <summary>
        /// Помечена ли сборка как удаленная.
        /// </summary>
        public bool IsUnTracked { get; set; }

        /// <inheritdoc />
        public bool Equals(BuildReadModel other)
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
                Id == other.Id &&
                DistributionId.Equals(other.DistributionId) &&
                LifeCycleState == other.LifeCycleState &&
                Tags.SequenceEqual(other.Tags) &&
                Suffixes.SequenceEqual(other.Suffixes) &&
                Number == other.Number &&
                ReleaseNumber == other.ReleaseNumber &&
                Location == other.Location &&
                BuildDate.Equals(other.BuildDate) &&
                IsUnTracked == other.IsUnTracked &&
                SourceType == other.SourceType;
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

            return Equals((BuildReadModel)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = DistributionId.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)LifeCycleState;
                hashCode = (hashCode * 397) ^ Tags.GetSequenceHashCode();
                hashCode = (hashCode * 397) ^ Suffixes.GetSequenceHashCode();
                hashCode = (hashCode * 397) ^ (Number != null ? Number.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ReleaseNumber != null ? ReleaseNumber.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Location != null ? Location.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ BuildDate.GetHashCode();
                hashCode = (hashCode * 397) ^ IsUnTracked.GetHashCode();
                hashCode = (hashCode * 397) ^ SourceType.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"{nameof(Number)}: {Number}, " +
            $"{nameof(SourceType)}: {SourceType}" + 
            $"{nameof(Location)}: {Location}" + 
            $"{nameof(DistributionId)}: {DistributionId}";
    }
}
