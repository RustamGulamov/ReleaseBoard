using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Builds.Mappers;
using ReleaseBoard.Domain.Builds.Settings;
using Xunit;

namespace ReleaseBoard.UnitTests.Domain.Builds
{
    /// <summary>
    /// Тесты для <see cref="BuildLifeCycleStateMapper"/>.
    /// </summary>
    public class BuildLifeCycleStateMapperTests
    {
        private readonly BuildLifeCycleStateMapper mapper = new BuildLifeCycleStateMapper(new BuildSettings()
            {
                SuffixToLifeCycleStateMapping = new Dictionary<string, LifeCycleState>()
                {
                    { "RC", LifeCycleState.ReleaseCandidate },
                    { "R", LifeCycleState.Release },
                    { "Second_Release_Suffix", LifeCycleState.Release }
                }
            });

        /// <summary>
        /// Коллекции для проверки маппера суффиксов на LifeCycleState.
        /// </summary>
        public static IEnumerable<object[]> SuffixesAndExceptedState => new[]
        {
            new object[] { new string[] { }, LifeCycleState.Build },
            new object[] { new string[] { null }, LifeCycleState.Build },
            new object[] { new string[] { "Not_Exists_Suffix", "LOL" }, LifeCycleState.Build },
            new object[] { new string[] { "R", "Not_Exists_Suffix" }, LifeCycleState.Release },
            new object[] { new string[] { "R", "Not_Exists_Suffix", "R" }, LifeCycleState.Release },
            new object[] { new string[] { "RC", "rC" }, LifeCycleState.ReleaseCandidate },
            new object[] { new string[] { "Second_Release_Suffix", "R" }, LifeCycleState.Release },
            new object[] { new string[] { "Not_Exists_Suffix", "R", "R" }, LifeCycleState.Release },
            new object[] { new string[] { "R", "RC" }, LifeCycleState.Release },
            new object[] { new string[] { "RC", "R" }, LifeCycleState.ReleaseCandidate },
            new object[] { new string[] { "RC", "Cert", "R" }, LifeCycleState.ReleaseCandidate },
        };

        /// <summary>
        /// Тест на маппинг суффиксов.
        /// </summary>
        /// <param name="suffixes">Суффиксы.</param>
        /// <param name="expectedState">Ожидаемый результат.</param>
        [Theory]
        [MemberData(nameof(SuffixesAndExceptedState))]
        public void Map_Suffixes_ExpectedState(string[] suffixes, LifeCycleState expectedState)
        {
            LifeCycleState state = mapper.MapFromSuffixes(suffixes);

            Assert.Equal(expectedState, state);
        }
    }
}
