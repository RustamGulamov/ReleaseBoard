using System;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase.Constants
{
    /// <summary>
    /// Класс с идентификаторами фейковых дистрибутивов.
    /// </summary>
    public static class FakeDistributionsIds
    {
        /// <summary>
        /// Идентификатор дистрибутива VipnetCsp.
        /// </summary>
        public static readonly Guid VipnetCsp = Guid.NewGuid();

        /// <summary>
        /// Идентификатор дистрибутива VipnetClient.
        /// </summary>
        public static readonly Guid VipnetClient = Guid.NewGuid();

        /// <summary>
        /// Идентификатор дистрибутива VipnetPkiClient.
        /// </summary>
        public static readonly Guid VipnetPkiClient = Guid.NewGuid();

        /// <summary>
        /// Идентификатор дистрибутива неизвестного проекта без сборок.
        /// </summary>
        public static readonly Guid Empty = Guid.NewGuid();

        /// <summary>
        /// Все известные идентификаторы известных фейковых дистрибутивов.
        /// </summary>
        public static readonly Guid[] AllAvailable = { VipnetCsp, VipnetClient, VipnetPkiClient, Empty };
    }
}
