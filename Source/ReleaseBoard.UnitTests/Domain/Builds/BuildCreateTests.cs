using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Events;
using ReleaseBoard.Domain.Builds.Exceptions;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Exceptions;
using ReleaseBoard.Domain.ValueObjects;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Builds
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public class BuildCreateTests : BuildTests
    {
        /// <summary>
        /// Тест на валидное создание аггрегата.
        /// </summary>
        [Fact]
        public void CreateAggregate_Valid()
        {
            var build = CreateBuild();

            Assert.NotNull(build);
            Assert.IsType<BuildCreated>(build.GetUncommitedChanges().First());
            Assert.NotEqual(build.Id, Guid.Empty);
            Assert.Equal(DistributionId, build.DistributionId);
            Assert.Equal(BuildDate, build.CreatedAt);
            Assert.Equal(Location, build.Location);
            Assert.Equal(Number, build.Number);
            Assert.Equal(ReleaseNumber, build.ReleaseNumber);
        }

        /// <summary>
        /// Тест на невалидные данные.
        /// </summary>
        [Fact]
        public void CreateBuild_InvalidData_ThrowExceptions()
        {
            Assert.Throws<CreateBuildException>(() =>
                new Build(BuildDate, Number, ReleaseNumber, DistributionId, string.Empty, BuildSourceType.Pdc ,LifeCycleState.Build, new List<string>()));
            Assert.Throws<CreateBuildException>(() =>
                new Build(BuildDate, Number, ReleaseNumber, DistributionId, null, BuildSourceType.Artifactory, LifeCycleState.Build, new List<string>()));
            Assert.Throws<DomainException>(() =>
                new Build(BuildDate, new VersionNumber("1.3.2.3"), new VersionNumber("1.5.2.3"), DistributionId, Location, BuildSourceType.Artifactory, LifeCycleState.Build, new List<string>()));
        }

        /// <summary>
        /// Тест на валидное создание аггрегата.
        /// </summary>
        [Fact]
        public void RestoreFromEventAggregate_Valid()
        {
            Guid buildId = Guid.NewGuid();
            Metadata metadata = new Metadata() { AggregateId = buildId };
            var build = new Build(new List<Event>()
            {
                new BuildCreated(buildId, BuildDate, DistributionId, Location, Number, ReleaseNumber,  LifeCycleState.Build, BuildSourceType.Artifactory, new string[] {}, metadata), 
                new ArtifactStateChanged(ArtifactState.Created, metadata)
            });

            Assert.NotNull(build);
            Assert.Empty(build.GetUncommitedChanges());
            Assert.NotEqual(build.Id, Guid.Empty);
            Assert.Equal(DistributionId, build.DistributionId);
            Assert.Equal(BuildDate, build.CreatedAt);
            Assert.Equal(Location, build.Location);
            Assert.Equal(ReleaseNumber, build.ReleaseNumber);
            Assert.Equal(Number, build.Number);
            Assert.Equal(ArtifactState.Created, build.ArtifactState);
            Assert.Equal(BuildSourceType.Artifactory, build.SourceType);
        }
    }
}
