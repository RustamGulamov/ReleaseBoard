using System;
using System.Collections.Generic;
using ReleaseBoard.Application.Specifications;
using ReleaseBoard.ReadModels;
using Xunit;

namespace ReleaseBoard.UnitTests.QueryHandlers.Builds.Specifications
{
    /// <summary>
    /// Тесты для спецификации <see cref="WithBuildNumbersSpecification"/>.
    /// </summary>
    public class WithBuildNumbersSpecificationTests
    {
        /// <summary>
        /// Список номеров билдов спецификации, сборки
        /// и ожидаемого результата проверки спецификации.
        /// </summary>
        public static IEnumerable<object[]> ReleaseNumbersWithExpectedResults => new[]
        {
            new object[]
            {
                new string[] { "3.2.0.4801" },
                "3.2.0.4801",
                true
            },
            new object[]
            {
                new string[] { "4.2.8.44963", "4.2.8.46116" },
                "4.2.8.46116",
                true
            },
            new object[]
            {
                new string[] { "31067" },
                "4.2.8.46116",
                false
            },
            new object[]
            {
                new string[] { "4.2.8.46116_R" },
                "4.2.8.46116",
                false
            },
            new object[]
            {
                new string[] { },
                "4.2.8.46116",
                false
            },
        };

        /// <summary>
        /// Проверяет работу спецификации при различных входных данных.
        /// </summary>
        /// <param name="specificationbuildNumbers">Номер версии билда спецификации.</param>
        /// <param name="buildNumber">Номер версии билда проверяемой сборки.</param>
        /// <param name="expectedResult">Ожидаемый результат проверки спецификации.</param>
        [Theory]
        [MemberData(nameof(ReleaseNumbersWithExpectedResults))]
        public void IsSatisfiedBy_BuildNumbers_ResultExpected(
            string[] specificationbuildNumbers,
            string buildNumber,
            bool expectedResult)
        {
            var build = new BuildReadModel { Number = buildNumber };
            var specification = new WithBuildNumbersSpecification(specificationbuildNumbers);

            var result = specification.IsSatisfiedBy(build);

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Проверяет генерацию исключения, когда в спецификацию передается неверный номер билдов.
        /// </summary>
        /// <param name="specificationbuildNumbers">Номера билдов спецификации.</param>
        [Theory]
        [InlineData(null)]
        public void IsSatisfiedBy_InvalidBuildNumbers_ThrowsArgumentNullException(string[] specificationbuildNumbers)
        {
            Assert.Throws<ArgumentNullException>(
                () => new WithBuildNumbersSpecification(specificationbuildNumbers));
        }
    }
}
