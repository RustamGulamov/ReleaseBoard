using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Common.Contracts.Extensions;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Application.QueryHandlers
{
    /// <summary>
    /// Генератор пути к сборокам.
    /// </summary>
    public static class BuildPathGenerator
    {
        private static readonly string point = ".";

        /// <summary>
        /// Генерировать все возможные относительные пути сборок по version.
        /// </summary>
        /// <param name="versionNumber">Версия сборки.</param>
        /// <param name="separator">Раздилитель.</param>
        /// <returns>Все возможные относительные пути.</returns>
        public static IEnumerable<string> GenerateAllPossibleRelativePath(VersionNumber versionNumber, char separator)
        {
            string versionNumberAsString = versionNumber.ToString();

            IEnumerable<string> buildNumbers = GenerateAllPossibleBuildNumbers(versionNumberAsString);
            IEnumerable<string> releaseNumbers = GenerateAllPossibleReleaseNumbers(versionNumberAsString);
            var paths = new HashSet<string>(buildNumbers);

            foreach (var releaseNumber in releaseNumbers)
            {
                foreach (var buildNumber in buildNumbers)
                {
                    paths.Add(string.Join(separator, releaseNumber, buildNumber));
                }
            }

            return paths;
        }

        private static IEnumerable<string> GenerateAllPossibleBuildNumbers(string versionNumber)
        {
            var result = new List<string>()
            {
                versionNumber
            };

            string[] numbers = versionNumber.Split(point);

            for (int i = 1; i < numbers.Length; i++)
            {
                string left = numbers.Take(i).JoinWith(point);
                string right = numbers.Skip(i).JoinWith(point);

                result.Add($"{left}_({right})");
            }

            return result;
        }

        private static IEnumerable<string> GenerateAllPossibleReleaseNumbers(string versionNumber)
        {
            var result = new List<string>();
            string[] numbers = versionNumber.Split(point);

            for (int i = 2; i < numbers.Length; i++)
            {
                string releaseNumber = numbers.Take(i).JoinWith(point);

                result.Add(releaseNumber);
            }

            return result;
        }
    }
}
