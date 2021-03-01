using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using ReleaseBoard.Application.Factories;
using ReleaseBoard.Application.Models;
using ReleaseBoard.Application.Models.Filters;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.Specifications;
using ReleaseBoard.ReadModels;
using Xunit;

namespace ReleaseBoard.UnitTests.QueryHandlers.Builds.Factories
{
    /// <summary>
    /// Класс с тестами для <see cref="DistributionBuildsSpecificationFactory"/>.
    /// </summary>
    public class DistributionBuildsSpecificationFactoryTests
    {
        private static readonly Guid fakeDistributionId = Guid.NewGuid();
        private static readonly string[] simpleTag = { "TEST" };
        private static readonly string[] releaseSuffix = { "R" };

        /// <summary>
        /// Возможные фильтры дистрибутивов.
        /// </summary>
        public static IEnumerable<object[]> DistributionBuildsFilters => new[]
        {
            // Satisfied by all parameters:
            new object[]
            {
                GetFilter(),
                GetBuild(),
                true
            },
            // Not satisfied by build date range:
            new object[]
            {
                GetFilter(),
                GetBuild(buildDate: new DateTime(2017, 1, 1)),
                false
            },
            // Not satisfied by lifecycle state:
            new object[]
            {
                GetFilter(),
                GetBuild(lifeCycleState: LifeCycleState.Certified),
                false
            },
            // Life cycle states filter will be ignored:
            new object[]
            {
                GetFilter(lifeCycleStates: new LifeCycleState[] {}),
                GetBuild(lifeCycleState: LifeCycleState.ReadyForArchiving),
                true
            },
            // Life tags and suffixes filter will be ignored:
            new object[]
            {
                GetFilter(tags: new string[] {}, suffixes: new string[] {}),
                GetBuild(tags: new string[] {"TAG"}, suffixes: new string[] {"SUFFIX"}),
                true
            }
        };

        /// <summary>
        /// Тест для проверки метода CreateFromFilter.
        /// </summary>
        /// <param name="filter"><see cref="DistributionBuildsFilter"/>.</param>
        /// <param name="entityToCheck"><see cref="BuildReadModel"/>.</param>
        /// <param name="expectedResult">Ожидаемый результат.</param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous unit test.</placeholder></returns>
        [Theory]
        [MemberData(nameof(DistributionBuildsFilters))]
        public async Task CreateFromFilter_IsSatisfiedBy_ResultExpected(
            DistributionBuildsFilter filter,
            BuildReadModel entityToCheck,
            bool expectedResult)
        {
            var mockDistro = new Mock<IReadOnlyRepository<DistributionReadModel>>();
            mockDistro
                .Setup(x => x.QueryAll(
                    It.IsAny<Expression<Func<DistributionReadModel, bool>>>(),
                    It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(new List<DistributionReadModel>());

            var distributionBuildsSpecificationFactory = new DistributionBuildsSpecificationFactory(mockDistro.Object);
            Specification<BuildReadModel> specification = await distributionBuildsSpecificationFactory.CreateFromFilter(filter, null, true);

            bool result = specification.IsSatisfiedBy(entityToCheck);

            Assert.Equal(expectedResult, result);
        }

        private static DistributionBuildsFilter GetFilter(
            Guid distributionId = default,
            string[] tags = default,
            string[] suffixes = default,
            SelectCondition tagsCondition = SelectCondition.Or,
            SelectCondition suffixesCondition = SelectCondition.Or,
            LifeCycleState[] lifeCycleStates = default,
            DateTime startDate = default,
            DateTime endDate = default)
        {
            return new DistributionBuildsFilter
            {
                DistributionId = distributionId != default ? distributionId : fakeDistributionId,
                Tags = tags ?? simpleTag,
                Suffixes = suffixes ?? releaseSuffix,
                SuffixesCondition = suffixesCondition,
                TagsCondition = tagsCondition,
                LifeCycleStates = lifeCycleStates ?? new LifeCycleState[] { LifeCycleState.Build },
                CreationDateRange = new DateRange
                {
                    StartDate = startDate != default ? startDate : new DateTime(2018, 1, 1),
                    EndDate = endDate != default ? endDate : DateTime.Today
                }
            };
        }

        private static BuildReadModel GetBuild(
            Guid distributionId = default,
            string[] tags = default,
            string[] suffixes = default,
            LifeCycleState lifeCycleState = LifeCycleState.Build,
            DateTime buildDate = default)
        {
            return new BuildReadModel
            {
                DistributionId = distributionId != default ? distributionId : fakeDistributionId,
                Tags = (tags ?? simpleTag).ToList(),
                Suffixes = (suffixes ?? releaseSuffix).ToList(),
                LifeCycleState = lifeCycleState,
                BuildDate = buildDate != default ? buildDate : new DateTime(2018, 1, 2)
            };
        }
    }
}
