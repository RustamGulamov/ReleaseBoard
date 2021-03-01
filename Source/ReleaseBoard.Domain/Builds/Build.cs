using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Domain.Builds.Events;
using ReleaseBoard.Domain.Builds.Exceptions;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Exceptions;
using ReleaseBoard.Domain.Core.Extensions;
using ReleaseBoard.Domain.Metadata;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Builds
{
    /// <summary>
    /// Сборка.
    /// </summary>
    public class Build : Aggregate
    {
        /// <summary>
        /// Конструктор. Восстановить состояние агрегата.
        /// </summary>
        /// <param name="events">События.</param>
        public Build(IEnumerable<Event> events)
        {
            Replay(events);
        }

        /// <summary>
        /// Конструктор. Создает новый агрегат.
        /// </summary>
        /// <param name="buildDate">Дата сборки.</param>
        /// <param name="number">Номер.</param>
        /// <param name="releaseNumber">Номер релиза.</param>
        /// <param name="distributionId">Идентификатор дистрибутива.</param>
        /// <param name="location">Местоположение.</param>
        /// <param name="sourceType">Тип хранилища.</param>
        /// <param name="lifeCycleState"><see cref="LifeCycleState"/>.</param>
        /// <param name="suffixes">Суффиксы.</param>
        public Build(
            DateTime buildDate,
            VersionNumber number,
            VersionNumber releaseNumber,
            Guid distributionId,
            string location,
            BuildSourceType sourceType,
            LifeCycleState lifeCycleState, 
            IList<string> suffixes = null)
        {
            location.ThrowIsNullOrWhiteSpace(new CreateBuildException("Invalid location value"));
            ValidateReleaseNumber(releaseNumber, number, location);

            Id = Guid.NewGuid();
            
            Apply(new BuildCreated(
                Id, 
                buildDate,
                distributionId, 
                location, 
                number, 
                releaseNumber, 
                lifeCycleState, 
                sourceType, 
                suffixes ?? new List<string>(), 
                CreateMetadata()));
            UpdateArtifactState(ArtifactState.Created);
        }

        /// <summary>
        /// Cостояние артефактов (файлов сборки).
        /// </summary>
        public ArtifactState ArtifactState { get; private set; }

        /// <summary>
        /// Дата сборки.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Идентификатор дистрибутива.
        /// </summary>
        public Guid DistributionId { get; private set; }

        /// <summary>
        /// Помечена ли сборка как "удаленная".
        /// </summary>
        public bool IsUnTracked => DistributionId == Guid.Empty;

        /// <summary>
        /// Cостояние сборки в жизненном цикле выпуска релиза.
        /// </summary>
        public LifeCycleState LifeCycleState { get; private set; }

        /// <summary>
        /// Тип хранилища.
        /// </summary>
        public BuildSourceType SourceType { get; private set; }

        /// <summary>
        /// Относительный путь в хранилище сборок.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Номер сборки.
        /// </summary>
        public VersionNumber Number { get; private set; }

        /// <summary>
        /// Основной номер версии релиза.
        /// Например при парсинге билда HW100\4.3.2\4.3_(2.3298), то ReleaseNumber=4.3.2.
        /// </summary>
        public VersionNumber ReleaseNumber { get; private set; }

        /// <summary>
        /// Суффиксы.
        /// </summary>
        public IList<string> Suffixes { get; private set; } = new List<string>();

        /// <summary>
        /// Теги.
        /// </summary>
        public IList<string> Tags { get; } = new List<string>();

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Location}, SourceType: {SourceType}, DistributionId: {DistributionId}";
        }

        /// <summary>
        /// Обновляет сборку.
        /// </summary>
        /// <param name="location">Новый путь.</param>
        /// <param name="number">Номер сборки.</param>
        /// <param name="releaseNumber">Номер релиза.</param>
        /// <param name="updatedSuffixes">Новые суффиксы.</param>
        /// <param name="changeDate">Дата изменения сборки.</param>
        public void Update(string location, VersionNumber releaseNumber, VersionNumber number,  IList<string> updatedSuffixes, DateTime changeDate)
        {
            location.ThrowIsNullOrWhiteSpace(new UpdateBuildException("Location is null or empty."));
            number.ThrowIfNull(new UpdateBuildException("Number must not be null"));
            ValidateReleaseNumber(releaseNumber, number, location);
                
            IList<string> newSuffixes = 
                Suffixes.ToHashSet().SetEquals(updatedSuffixes)
                    ? Suffixes
                    : updatedSuffixes;

            if (location == Location && newSuffixes.SequenceEqual(Suffixes))
            {
                return;
            }
            
            Apply(new BuildUpdated(location, number, releaseNumber, newSuffixes, CreateMetadata(changeDate)));
            UpdateArtifactState(ArtifactState.Updated);
        }

        /// <summary>
        /// Помечать сборку как удалена.
        /// </summary>
        /// <param name="unTrackedDate">Дата удаления.</param>
        public void MarkAsUnTracked(DateTime unTrackedDate)
        {
            if (IsUnTracked)
            {
                return;
            }

            Apply(new BuildMarkAsUnTracked(DistributionId, CreateMetadata(unTrackedDate)));
            UpdateArtifactState(ArtifactState.UnTracked);
        }

        /// <summary>
        /// Помечать сборку как отслеживается.
        /// </summary>
        /// <param name="distributionId">Идентфикатор дистрибутива.</param>
        /// <param name="trackedDate">Дата изменения.</param>
        public void MarkAsTracked(Guid distributionId, DateTime trackedDate)
        {
            if (!IsUnTracked || distributionId == Guid.Empty)
            {
                return;
            }

            Apply(new BuildMarkAsTracked(Location, distributionId, CreateMetadata(trackedDate)));
            UpdateArtifactState(ArtifactState.ReturnToTracked);
        }
        
        /// <summary>
        /// Обновляет текущее состояние артефактов.
        /// </summary>
        /// <param name="artifactState">Новое состояние.</param>
        public void UpdateArtifactState(ArtifactState artifactState)
        {
            Apply(new ArtifactStateChanged(artifactState, CreateMetadata()));
        }

        /// <summary>
        /// Добавляет тег.
        /// </summary>
        /// <param name="tag">Новый тег.</param>
        public void AddTag(string tag)
        {
            tag.ThrowIsNullOrWhiteSpace(new UpdateBuildException("Tag is null."));

            if (Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }

            Apply(new BuildTagAdded(tag, CreateMetadata()));
        }

        /// <summary>
        /// Удаляет тег.
        /// </summary>
        /// <param name="tag">Тег.</param>
        public void RemoveTag(string tag)
        {
            tag.ThrowIsNullOrWhiteSpace(new UpdateBuildException("Tag is null."));

            if (!Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            {
                return;
            }

            Apply(new BuildTagRemoved(tag, CreateMetadata()));
        }

        /// <summary>
        /// Обновляет <see cref="LifeCycleState"/>.
        /// </summary>
        /// <param name="newState">Новое состояние билда.</param>
        /// <param name="changeDate">Дата изменения.</param>
        /// <param name="comment">Комментарий пользовталя.</param>
        /// <param name="actionUserId">Идентификатор пользователь, кто совершал действие.</param>
        public void UpdateLifeCycleState(
            LifeCycleState newState,
            DateTime changeDate,
            string comment = default,
            string actionUserId = default)
        {
            if (newState == LifeCycleState)
            {
                return;
            }

            var metadata = CreateMetadata(changeDate, actionUserId);
            if (!string.IsNullOrEmpty(comment))
            {
                metadata.Add(MetadataKeys.Comment, comment);
            }

            Apply(new LifeCycleStateChanged(newState, metadata));
            UpdateArtifactState(ArtifactState.Updated);
        }
        
        /// <inheritdoc/>
        protected override void Mutate(Event @event)
        {
            ((dynamic)this).When((dynamic)@event);
        }

        private void ValidateReleaseNumber(VersionNumber releaseNumber, VersionNumber number, string location)
        {
            if (releaseNumber != null && !releaseNumber.IsInclude(number))
            {
                throw new DomainException($"Release number {releaseNumber} not include build number {number}. Location: {location}.");
            }
        }

        private void When(BuildCreated @event)
        {
            Id = @event.Id;
            CreatedAt = @event.BuildDate;
            Number = @event.Number;
            ReleaseNumber = @event.ReleaseNumber;
            Location = @event.Location;
            DistributionId = @event.DistributionId;
            LifeCycleState = @event.LifeCycleState;
            SourceType = @event.SourceType;
            Suffixes = @event.Suffixes;
        }

        private void When(BuildUpdated @event)
        {
            (string location, VersionNumber number, VersionNumber releaseNumber, IList<string> suffixes, _) = @event;
            Location = location;
            Suffixes = suffixes;
            Number = number;
            ReleaseNumber = releaseNumber;
        }
        
        private void When(ArtifactStateChanged @event)
        {
            ArtifactState = @event.ArtifactState;
        }

        private void When(LifeCycleStateChanged @event)
        {
            LifeCycleState = @event.LifeCycleState;
        }

        private void When(BuildTagAdded @event)
        {
            Tags.Add(@event.Tag);
        }

        private void When(BuildTagRemoved @event)
        {
            Tags.Remove(@event.Tag);
        }

        private void When(BuildMarkAsTracked @event)
        {
            DistributionId = @event.DistributionId;
        }

        private void When(BuildMarkAsUnTracked @event)
        {
            DistributionId = Guid.Empty;
        }
    }
}
