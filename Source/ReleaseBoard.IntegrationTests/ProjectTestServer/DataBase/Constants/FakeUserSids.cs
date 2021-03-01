using System;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer.DataBase.Constants
{
    /// <summary>
    /// Идентификаторы фейковых пользователей.
    /// </summary>
    public class FakeUserSids
    {
        /// <summary>
        /// Kurindin. Начальник отдела ОВА.
        /// </summary>
        public const string Kurindin = "S-1-5-21-1128627858-1011171821-2048475684-9541";

        /// <summary>
        /// Medvedev. Зам. начальник отдела ОВА.
        /// </summary>
        public const string Medvedev = "S-1-5-21-1128627858-1011171821-2048475684-7115";

        /// <summary>
        /// Yakovlev. Ведущий программист ОВА.
        /// </summary>
        public const string Yakovlev = "S-1-5-21-1128627858-1011171821-2048475684-25637";

        /// <summary>
        /// Shagbalov. Программист ОВА.
        /// </summary>
        public const string Shagbalov = "S-1-5-21-1128627858-1011171821-2048475684-27737";

        /// <summary>
        /// Zhumikov. Программист ОВА.
        /// </summary>
        public const string Zhumikov = "S-1-5-21-1128627858-1011171821-2048475684-41335";

        /// <summary>
        /// Milich. Начальник отдела.
        /// </summary>
        public const string Milich = "S-1-5-21-1128627858-1011171821-2048475684-8359";

        /// <summary>
        /// UknownUser1.
        /// </summary>
        public const string UknownUser1 = "S-2-8-30-1128627858-1001191871-1043475684-47900";

        /// <summary>
        /// UknownUser2.
        /// </summary>
        public static readonly string UknownUser2 = Guid.NewGuid().ToString();
    }
}
