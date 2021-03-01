using System;
using System.Linq;
using AutoFixture;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase.Constants;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.IntegrationTests.AutoFixture.Customizations
{
    /// <summary>
    /// Кастомизация AutoFixture для генерации сборок.
    /// </summary>
    public class BuildsCustomization : ICustomization
    {
        private readonly Random random = new Random();
        private readonly Guid[] distributionsWithBuilds = FakeDistributionsIds
            .AllAvailable
            .Where(x => x != FakeDistributionsIds.Empty)
            .ToArray();
        
        /// <inheritdoc />
        public void Customize(IFixture fixture)
        {
            fixture.Customize<BuildReadModel>(b => b
                .WithAutoProperties()
                .With(x => x.LifeCycleState, () => fixture.Create<LifeCycleState>())
                .With(x => x.DistributionId, () => GetRandomValue(distributionsWithBuilds))
                .With(x => x.Tags, () => GetRandomValues(FakeTags.AllAvailable).ToList())
                .With(x => x.Suffixes, () => GetRandomValues(FakeSuffixes.AllAvailable).ToList())
                .With(x => x.ReleaseNumber, () => GetRandomValue(FakeReleaseNumbers.AllAvailable))
                .With(x => x.BuildDate, () => GetRandomBuildDate())
            );
        }

        private T[] GetRandomValues<T>(T[] values)
        {
            int countToTake = random.Next(0, values.Length);
            return values.Take(countToTake).ToArray();
        }

        private T GetRandomValue<T>(T[] values)
        {
            int index = random.Next(0, values.Length - 1);
            return values[index];
        }

        private DateTime GetRandomBuildDate()
        {
            TimeSpan rangeDifferenceTimeSpan = FakeBuildDatesRange.EndDate - FakeBuildDatesRange.StartDate;
            int additionalHours = random.Next(0, (int)rangeDifferenceTimeSpan.TotalMinutes);
            TimeSpan additionalTimeSpan = new TimeSpan(0, additionalHours, 0);
            return FakeBuildDatesRange.StartDate + additionalTimeSpan;
        }
    }
}
