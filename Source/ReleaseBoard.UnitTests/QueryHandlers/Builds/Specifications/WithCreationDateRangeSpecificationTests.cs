using System;
using System.Collections.Generic;
using ReleaseBoard.Application.Models;
using ReleaseBoard.Application.Specifications;
using ReleaseBoard.ReadModels;
using Xunit;

namespace ReleaseBoard.UnitTests.QueryHandlers.Builds.Specifications
{
    /// <summary>
    /// Тесты для спецификации <see cref="WithCreationDateRangeSpecification"/>.
    /// </summary>
    public class WithCreationDateRangeSpecificationTests
    {
        /// <summary>
        /// Список дат начала/окончания диапазона, сборки и ожидаемого результата проверки спецификации.
        /// </summary>
        public static IEnumerable<object[]> DatesWithExpectedResults => new[]
        {
            new object[]
            {
                new DateTime(2018, 1, 1),
                new DateTime(2018, 1, 2),
                new DateTime(2018, 1, 1),
                true
            },
            new object[]
            {
                new DateTime(2018, 1, 1),
                new DateTime(2019, 1, 2),
                new DateTime(2018, 12, 31),
                true
            },
            new object[]
            {
                new DateTime(2018, 1, 1),
                new DateTime(2018, 1, 1),
                new DateTime(2018, 1, 1, 0, 0, 1),
                true
            },
            new object[]
            {
                new DateTime(2020, 1, 1),
                new DateTime(2019, 1, 1),
                new DateTime(2018, 1, 1),
                false
            },
            new object[]
            {
                new DateTime(2018, 1, 1),
                new DateTime(2019, 1, 1),
                new DateTime(2020, 1, 1),
                false
            }
        };

        /// <summary>
        /// Проверяет работу спецификации при различных входных данных.
        /// </summary>
        /// <param name="startRange">Дата начала диапазона.</param>
        /// <param name="endRange">Дата окончания диапазона.</param>
        /// <param name="buildDate">Дата сборки.</param>
        /// <param name="expectedResult">Ожидаемый результат проверки спецификации.</param>
        [Theory]
        [MemberData(nameof(DatesWithExpectedResults))]
        private void IsSatisfiedBy_BuildDateInsideRange_ResultExpected(
            DateTime startRange,
            DateTime endRange,
            DateTime buildDate,
            bool expectedResult)
        {
            var build = new BuildReadModel { BuildDate = buildDate };
            var range = new DateRange { StartDate = startRange, EndDate = endRange };
            var specification = new WithCreationDateRangeSpecification(range);

            var result = specification.IsSatisfiedBy(build);

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Проверяет генерацию исключения при неверном аргументе конструктора.
        /// </summary>
        [Fact]
        private void IsSatisfiedBy_InvalidDateRange_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new WithCreationDateRangeSpecification(null));
        }
    }
}
