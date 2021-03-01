using System;
using System.Collections.Generic;
using ReleaseBoard.Web.ApiModels.DistributionModels;
using Xunit;

namespace ReleaseBoard.UnitTests.ReadModels.EqualityComparers
{
    /// <summary>
    /// Класс с тестами для <see cref="ProjectBindingModel"/>.
    /// </summary>
    public class ProjectBindingModelComparerTests
    {
        private static readonly Guid someProjectId = Guid.Parse("f6aba509-c148-4d26-867d-168775ee77c7");

        /// <summary>
        /// Список привязок к проектам для сравнения с ожидаемыми результатами сравнения.
        /// </summary>
        public static IEnumerable<object[]> ProjectBindingModelsWithExpectedResults => new[]
        {
            new object[] { null, null, true },
            new object[] { CreateProjectBindingModel(), CreateProjectBindingModel(), true },
            new object[]
            {
                CreateProjectBindingModel(maskProjectVersion: nameof(CreateProjectBindingModel)), 
                CreateProjectBindingModel(maskProjectVersion: nameof(CreateProjectBindingModel)), 
                true
            },

            new object[] { null, CreateProjectBindingModel(), false },
            new object[] { CreateProjectBindingModel(), null, false },

            new object[] { CreateProjectBindingModel(someProjectId), CreateProjectBindingModel(), false },
            new object[] { CreateProjectBindingModel(), CreateProjectBindingModel(someProjectId), false },

            new object[] { CreateProjectBindingModel(maskProjectVersion: nameof(CreateProjectBindingModel)), CreateProjectBindingModel(), false },
            new object[] { CreateProjectBindingModel(), CreateProjectBindingModel(maskProjectVersion: nameof(CreateProjectBindingModel)), false },
        };

        /// <summary>
        /// Проверяет сравнение двух привязок к проектам посредством метода Equals.
        /// </summary>
        /// <param name="x">Первый объект для сравнения.</param>
        /// <param name="y">Второй объект для сравнения.</param>
        /// <param name="expectedResult">Ожидаемый результат сравнения.</param>
        [Theory]
        [MemberData(nameof(ProjectBindingModelsWithExpectedResults))]
        public void Equals_ProejctBindingModels_ResultExpected(ProjectBindingModel x, ProjectBindingModel y, bool expectedResult)
        {
            Assert.Equal(expectedResult, Equals(x, y));
        }

        private static ProjectBindingModel CreateProjectBindingModel(
            Guid projectId = default, string maskProjectVersion = default) =>
            new ProjectBindingModel { ProjectId = projectId, MaskProjectVersion = maskProjectVersion };
    }
}
