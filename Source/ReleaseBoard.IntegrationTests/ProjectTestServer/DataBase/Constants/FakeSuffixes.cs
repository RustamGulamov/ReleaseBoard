using System;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase.Constants
{
    /// <summary>
    /// Константы фейковых суффиксов.
    /// </summary>
    public static class FakeSuffixes
    {
        /// <summary>
        /// Суффикс - релиз.
        /// </summary>
        public const string R = nameof(R);

        /// <summary>
        /// Суффикс - релиз кандидат.
        /// </summary>
        public const string RC = nameof(RC);

        /// <summary>
        /// Суффикс - неофициальная передача.
        /// </summary>
        public const string N = nameof(N);

        /// <summary>
        /// Все известные фейковые суффиксы.
        /// </summary>
        public static readonly string[] AllAvailable = { R, RC, N };
    }
}
