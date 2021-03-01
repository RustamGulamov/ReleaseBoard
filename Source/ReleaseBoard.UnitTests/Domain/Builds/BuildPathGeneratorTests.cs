using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Application.QueryHandlers;
using ReleaseBoard.Domain.ValueObjects;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Builds
{
    /// <summary>
    /// Тесты для класса BuildPathGenerator.
    /// </summary>
    public class BuildPathGeneratorTests
    {
        /// <summary>
        /// Коллекции для проверки генератора для вычисления пути.
        /// </summary>
        public static IEnumerable<object[]> VersionAndExpectedRelativePaths => new[]
        {
            new object[]
            {
                "1.2.3",
                BuildSourceType.Pdc,
                new string[]
                {
                    "1.2.3",
                    "1_(2.3)",
                    "1.2_(3)",
                    "1.2\\1.2.3",
                    "1.2\\1_(2.3)",
                    "1.2\\1.2_(3)"
                }
            },
            new object[]
            {
                "1.2.3",
                BuildSourceType.Artifactory,
                new string[]
                {
                    "1.2.3",
                    "1_(2.3)",
                    "1.2_(3)",
                    "1.2/1.2.3",
                    "1.2/1_(2.3)",
                    "1.2/1.2_(3)"
                }
            },
            new object[]
            {
                "1.2.3.4",
                BuildSourceType.Pdc,
                new string[]
                {
                    "1.2.3.4",
                    "1_(2.3.4)",
                    "1.2_(3.4)",
                    "1.2.3_(4)",
                    "1.2\\1.2.3.4",
                    "1.2\\1_(2.3.4)",
                    "1.2\\1.2_(3.4)",
                    "1.2\\1.2.3_(4)",
                    "1.2.3\\1.2.3.4",
                    "1.2.3\\1_(2.3.4)",
                    "1.2.3\\1.2_(3.4)",
                    "1.2.3\\1.2.3_(4)",
                }
            },
        };

        /// <summary>
        /// Тест проверяет все возможные относительные пути сборок по version.
        /// </summary>
        /// <param name="version">Версия.</param>
        /// <param name="sourceType"><see cref="BuildSourceType"/>.</param>
        /// <param name="exceptedResult">Ожидаемый результат.</param>
        [Theory]
        [MemberData(nameof(VersionAndExpectedRelativePaths))]
        public void GenerateAllPossibleRelativePath_EqualsToExpectedResult(string version, BuildSourceType sourceType, string[] exceptedResult)
        {
            var versionNumber = new VersionNumber(version);

            string[] relativePaths = BuildPathGenerator.GenerateAllPossibleRelativePath(versionNumber, GetSeparator(sourceType)).ToArray();

            Assert.Equal(exceptedResult, relativePaths);
        }

        private char GetSeparator(BuildSourceType buildSourceType)
        {
            switch (buildSourceType)
            {
                case BuildSourceType.Pdc: return Path.DirectorySeparatorChar;
                case BuildSourceType.Artifactory: return Path.AltDirectorySeparatorChar;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
