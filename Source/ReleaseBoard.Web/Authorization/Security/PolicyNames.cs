using System;

namespace ReleaseBoard.Web.Authorization.Security
{
    /// <summary>
    /// Названия политик безопасности.
    /// </summary>
    public class PolicyNames
    {
        /// <summary>
        /// Управление дистрибутивами (создание/обновление и т.п.).
        /// </summary>
        public const string CanManageDistributions = nameof(CanManageDistributions);

        /// <summary>
        /// Доступ к API, нужного для BuildSync'а.
        /// </summary>
        public const string CanAccessBuildSyncApi = nameof(CanAccessBuildSyncApi);
    }
}
