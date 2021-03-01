using System;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase.Constants
{
    /// <summary>
    /// Константы фейковых тегов.
    /// </summary>
    public class FakeTags
    {
        /// <summary>
        /// CERT.
        /// </summary>
        public const string CERT = nameof(CERT);

        /// <summary>
        /// AWESOME.
        /// </summary>
        public const string AWESOME = nameof(AWESOME);

        /// <summary>
        /// Все известные фейковые суффиксы.
        /// </summary>
        public static readonly string[] AllAvailable = { CERT, AWESOME };
    }
}
