using System;
using System.Collections.Generic;
using ReleaseBoard.Web.ApiModels.DistributionModels;
using Xunit;

namespace ReleaseBoard.UnitTests.ReadModels.EqualityComparers
{
    /// <summary>
    /// Класс с тестами для <see cref="BuildBindingModel"/>.
    /// </summary>
    public class BuildBindingModelComparerTests
    {
        /// <summary>
        /// Список привязок к сборкам для сравнения с ожидаемыми результатами сравнения.
        /// </summary>
        public static IEnumerable<object[]> BuildBindingModelsWithExpectedResults => new[]
        {
            new object[] { null, null, true },
            new object[] { CreateBuildBindingModel(), CreateBuildBindingModel(), true },
            new object[] { CreateBuildBindingModel(path: nameof(CreateBuildBindingModel)), CreateBuildBindingModel(path: nameof(CreateBuildBindingModel).ToUpper()), true },

            new object[] { null, CreateBuildBindingModel(), false },
            new object[] { CreateBuildBindingModel(), null, false },

            new object[] { CreateBuildBindingModel(Guid.NewGuid()), CreateBuildBindingModel(), false },
            new object[] { CreateBuildBindingModel(), CreateBuildBindingModel(Guid.NewGuid()), false },

            new object[] { CreateBuildBindingModel(path: nameof(CreateBuildBindingModel)), CreateBuildBindingModel(), false },
            new object[] { CreateBuildBindingModel(), CreateBuildBindingModel(path: nameof(CreateBuildBindingModel)), false },
        };

        /// <summary>
        /// Проверяет сравнение двух привязок к сборкам посредством метода Equals.
        /// </summary>
        /// <param name="x">Первый объект для сравнения.</param>
        /// <param name="y">Второй объект для сравнения.</param>
        /// <param name="expectedResult">Ожидаемый результат сравнения.</param>
        [Theory]
        [MemberData(nameof(BuildBindingModelsWithExpectedResults))]
        public void Equals_BuildBindingModels_ResultExpected(BuildBindingModel x, BuildBindingModel y, bool expectedResult)
        {
            Assert.Equal(expectedResult, Equals(x, y));
        }

        private static BuildBindingModel CreateBuildBindingModel(
            Guid patternId = default, string path = default) =>
                new BuildBindingModel { PatternId = patternId, Path = path };
    }
}
