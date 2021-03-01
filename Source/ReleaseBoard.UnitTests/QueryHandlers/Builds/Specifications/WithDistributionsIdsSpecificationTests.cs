using System;
using System.Collections.Generic;
using ReleaseBoard.Application.Specifications;
using ReleaseBoard.ReadModels;
using Xunit;

namespace ReleaseBoard.UnitTests.QueryHandlers.Builds.Specifications
{
    /// <summary>
    /// Тесты для спецификации <see cref="WithDistributionsIdsSpecification"/>.
    /// </summary>
    public class WithDistributionsIdsSpecificationTests
    {
        private static readonly Guid fakeDistributionId = Guid.NewGuid();

        /// <summary>
        /// Список суффиксов фильтра, операции сравнения,
        /// сборки и ожидаемого результата проверки спецификации.
        /// </summary>
        public static IEnumerable<object[]> DistributionsIdsWithExpectedResults => new[]
        {
            new object[]
            {
                fakeDistributionId,
                fakeDistributionId,
                true
            },
            new object[]
            {
                Guid.Empty,
                Guid.Empty,
                true
            },
            new object[]
            {
                fakeDistributionId,
                Guid.Empty,
                false
            },
            new object[]
            {
                Guid.Empty,
                fakeDistributionId,
                false
            },
            new object[]
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                false
            }
        };

        /// <summary>
        /// Проверяет работу спецификации при различных входных данных.
        /// </summary>
        /// <param name="specificationDistributionId">Идентификатор дистрибутива спецификации.</param>
        /// <param name="buildDistributionId">Идентификатор дистрибутива у проверяемой сборки.</param>
        /// <param name="expectedResult">Ожидаемый результат проверки спецификации.</param>
        [Theory]
        [MemberData(nameof(DistributionsIdsWithExpectedResults))]
        private void IsSatisfiedBy_BuildDateWithRange_ResultExpected(
            Guid specificationDistributionId,
            Guid buildDistributionId,
            bool expectedResult)
        {
            var build = new BuildReadModel { DistributionId = buildDistributionId };
            var specification = new WithDistributionsIdsSpecification(specificationDistributionId);

            var result = specification.IsSatisfiedBy(build);

            Assert.Equal(expectedResult, result);
        }
    }
}

