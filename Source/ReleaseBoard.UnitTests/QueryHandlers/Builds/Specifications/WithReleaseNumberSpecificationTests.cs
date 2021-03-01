using System;
using System.Collections.Generic;
using ReleaseBoard.Application.Specifications;
using ReleaseBoard.ReadModels;
using Xunit;

namespace ReleaseBoard.UnitTests.QueryHandlers.Builds.Specifications
{
    /// <summary>
    /// Тесты для спецификации <see cref="WithReleaseNumberSpecification"/>.
    /// </summary>
    public class WithReleaseNumberSpecificationTests
    {
        private const string FakeReleaseNumber = "4.2.2";

        /// <summary>
        /// Список номеров релиза спецификации, сборки
        /// и ожидаемого результата проверки спецификации.
        /// </summary>
        public static IEnumerable<object[]> ReleaseNumbersWithExpectedResults => new[]
        {
            new object[]
            {
                FakeReleaseNumber,
                FakeReleaseNumber,
                true
            },
            new object[]
            {
                FakeReleaseNumber,
                Guid.NewGuid().ToString(),
                false
            },
        };

        /// <summary>
        /// Проверяет работу спецификации при различных входных данных.
        /// </summary>
        /// <param name="specificationReleaseNumber">Номер версии релиза спецификации.</param>
        /// <param name="buildReleaseNumber">Номер версии релиза проверяемой сборки.</param>
        /// <param name="expectedResult">Ожидаемый результат проверки спецификации.</param>
        [Theory]
        [MemberData(nameof(ReleaseNumbersWithExpectedResults))]
        private void IsSatisfiedBy_ReleaseNumbers_ResultExpected(
            string specificationReleaseNumber,
            string buildReleaseNumber,
            bool expectedResult)
        {
            var build = new BuildReadModel { ReleaseNumber = buildReleaseNumber };
            var specification = new WithReleaseNumberSpecification(specificationReleaseNumber);

            var result = specification.IsSatisfiedBy(build);

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Проверяет генерацию исключения, когда в спецификацию передается неверный номер релиза.
        /// </summary>
        /// <param name="specificationReleaseNumber">Номер версии релиза спецификации.</param>
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        private void IsSatisfiedBy_InvalidReleaseNumber_ThrowsArgumentNullException(string specificationReleaseNumber)
        {
            Assert.Throws<ArgumentNullException>(
                () => new WithReleaseNumberSpecification(specificationReleaseNumber));
        }
    }
}
