using System;
using System.Collections.Generic;
using ReleaseBoard.Application.Specifications;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.ReadModels;
using Xunit;

namespace ReleaseBoard.UnitTests.QueryHandlers.Builds.Specifications
{
    /// <summary>
    /// Тесты для спецификации <see cref="WithLifeCycleStateSpecification"/>.
    /// </summary>
    public class WithLifeCycleStateSpecificationTests
    {
        private static readonly LifeCycleState[] fakeLifeCycleStates =
        {
            LifeCycleState.Build,
            LifeCycleState.Certified
        };

        /// <summary>
        /// Список <see cref="LifeCycleState"/> спецификации, сборки
        /// и ожидаемого результата проверки спецификации.
        /// </summary>
        public static IEnumerable<object[]> LifeCycleStatesWithExpectedResults => new[]
        {
            new object[]
            {
                fakeLifeCycleStates,
                LifeCycleState.Build,
                true
            },
            new object[]
            {
                fakeLifeCycleStates,
                LifeCycleState.Certified,
                true
            },
            new object[]
            {
                fakeLifeCycleStates,
                LifeCycleState.Release,
                false
            },
            new object[]
            {
                new LifeCycleState[] { },
                LifeCycleState.Build,
                false
            },
        };

        /// <summary>
        /// Проверяет работу спецификации при различных входных данных.
        /// </summary>
        /// <param name="specificationLifeCycleStates">Список <see cref="LifeCycleState"/> спецификации.</param>
        /// <param name="buildLifeCycleState"><see cref="LifeCycleState"/> у проверяемой сборки.</param>
        /// <param name="expectedResult">Ожидаемый результат проверки спецификации.</param>
        [Theory]
        [MemberData(nameof(LifeCycleStatesWithExpectedResults))]
        public void IsSatisfiedBy_LifeCycleStates_ResultExpected(
            LifeCycleState[] specificationLifeCycleStates,
            LifeCycleState buildLifeCycleState,
            bool expectedResult)
        {
            var build = new BuildReadModel { LifeCycleState = buildLifeCycleState };
            var specification = new WithLifeCycleStateSpecification(specificationLifeCycleStates);

            var result = specification.IsSatisfiedBy(build);

            Assert.Equal(expectedResult, result);
        }
    }
}
