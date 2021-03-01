using System;
using Hangfire.Dashboard;

namespace ReleaseBoard.Web.Authorization
{
    /// <summary>
    /// Фильтр для авторизации на hangfire dashboard с любого хоста.
    /// </summary>
    public class HangfireAnonymousAuthorizationFilter: IDashboardAuthorizationFilter
    {
        /// <inheritdoc />
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}
