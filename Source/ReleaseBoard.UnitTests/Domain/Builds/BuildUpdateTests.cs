using System;
using System.Linq;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Events;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Builds
{
    /// <summary>
    /// Тесты на обновление билда.
    /// </summary>
    public class BuildUpdateTests : BuildTests
    {
        /// <summary>
        /// Тест на обновление состояния артефакта..
        /// </summary>
        [Fact]
        public void UpdateArtifactState_BuildValid_StateChanged()
        {
            var build = CreateBuild();
            build.UpdateArtifactState(ArtifactState.Updated);

            Assert.IsType<ArtifactStateChanged>(build.GetUncommitedChanges().Last());
            Assert.Equal(ArtifactState.Updated, build.ArtifactState);
        }
    }
}
