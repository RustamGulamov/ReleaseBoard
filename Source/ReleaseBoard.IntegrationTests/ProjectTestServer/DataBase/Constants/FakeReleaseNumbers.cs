using System;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase.Constants
{
    /// <summary>
    /// Класс с фейковыми номерами релизов.
    /// </summary>
    public static class FakeReleaseNumbers
    {
        /// <summary>
        /// Версия релиза 1.0.0.
        /// </summary>
        public const string R100 = "1.0.0";

        /// <summary>
        /// Версия релиза 1.0.1.
        /// </summary>
        public const string R101 = "1.0.1";

        /// <summary>
        /// Версия релиза 1.0.2.
        /// </summary>
        public const string R102 = "1.0.2";

        /// <summary>
        /// Версия релиза 1.1.0.
        /// </summary>
        public const string R110 = "1.1.0";

        /// <summary>
        /// Версия релиза 1.1.1.
        /// </summary>
        public const string R111 = "1.1.1";

        /// <summary>
        /// Версия релиза 1.1.2.
        /// </summary>
        public const string R112 = "1.1.2";

        /// <summary>
        /// Все известные номера фейковых релизов.
        /// </summary>
        public static readonly string[] AllAvailable = { R100, R101, R102, R110, R111, R112 };
    }
}
