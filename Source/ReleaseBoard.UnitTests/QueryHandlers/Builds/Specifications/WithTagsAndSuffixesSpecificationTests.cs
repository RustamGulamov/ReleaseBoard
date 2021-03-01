using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Application.Specifications;
using ReleaseBoard.Domain.Specifications;
using ReleaseBoard.ReadModels;
using Xunit;

namespace ReleaseBoard.UnitTests.QueryHandlers.Builds.Specifications
{
    /// <summary>
    /// Тесты для спецификаций <see cref="WithTagsSpecification"/> и <see cref="WithSuffixesSpecification"/>.
    /// </summary>
    public class WithTagsAndSuffixesSpecificationsTests
    {
        private static readonly string[] emptyArray = { };

        /// <summary>
        /// Список суффиксов фильтра, операции сравнения,
        /// сборки и ожидаемого результата проверки спецификации.
        /// </summary>
        public static IEnumerable<object[]> TagsAndSuffixesWithExpectedResults => new[]
        {
            // Satisfied by tag:
            new object[]
            {
                new string[] { "Tag" },
                SelectCondition.Or,
                new string[] { "Tag" },
                true
            },
            // Satisfied by at least one tag
            new object[]
            {
                new string[] { "tag", "tag2" },
                SelectCondition.Or,
                new string[] { "tag" },
                true
            },
            // Satisfied
            new object[]
            {
                new string[] { "tag", "tag2" },
                SelectCondition.And,
                new string[] { "tag", "tag2", "tag3" },
                true
            },
            // Not satisfied by all tags:
            new object[]
            {
                new string[] {"tags", "tags2" },
                SelectCondition.And,
                new string[] { "tags" },
                false
            },
            // Not satisfied by tag:
            new object[]
            {
                new string[] { "R" },
                SelectCondition.And,
                new string[] { "SomeTag" },
                false
            }
        };

        /// <summary>
        /// Проверяет работу спецификации тэгов при различных входных данных.
        /// </summary>
        /// <param name="specificationTags">Список тегов фильтра.</param>
        /// <param name="tagsCondition">Условие подбора тегов  <see cref="SelectCondition"/>.</param>
        /// <param name="buildTags">Список тегов сборки.</param>
        /// <param name="expectedResult">Ожидаемый результат проверки спецификации.</param>
        [Theory]
        [MemberData(nameof(TagsAndSuffixesWithExpectedResults))]
        public void IsSatisfiedBy_WithTags_ResultExpected(
            string[] specificationTags,
            SelectCondition tagsCondition,
            string[] buildTags,
            bool expectedResult)
        {
            var build = new BuildReadModel { Tags = buildTags.ToList() };
            var specification = new WithTagsSpecification(specificationTags, tagsCondition);

            var result = specification.IsSatisfiedBy(build);

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Проверяет работу спецификации суффиксов при различных входных данных.
        /// </summary>
        /// <param name="specificationSuffixes">Список суффиксов фильтра.</param>
        /// <param name="condition">Условие подбора суффиксов <see cref="SelectCondition"/>.</param>
        /// <param name="buildSuffixes">Список суффиксов сборки.</param>
        /// <param name="expectedResult">Ожидаемый результат проверки спецификации.</param>
        [Theory]
        [MemberData(nameof(TagsAndSuffixesWithExpectedResults))]
        private void IsSatisfiedBy_WithSuffixes_ResultExpected(
            string[] specificationSuffixes,
            SelectCondition condition,
            string[] buildSuffixes,
            bool expectedResult)
        {
            var build = new BuildReadModel { Suffixes = buildSuffixes.ToList() };
            var specification = new WithSuffixesSpecification(specificationSuffixes, condition);

            var result = specification.IsSatisfiedBy(build);

            Assert.Equal(expectedResult, result);
        }

        /// <summary>
        /// Проверяет генерацию исключения, когда в спецификацию
        /// передается неверный объект.
        /// </summary>
        [Fact]
        private void IsSatisfiedBy_InvalidCondition_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new WithTagsSpecification(null, SelectCondition.And));
        }
    }
}
