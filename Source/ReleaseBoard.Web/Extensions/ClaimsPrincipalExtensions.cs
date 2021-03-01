using System;
using System.Security.Claims;
using ReleaseBoard.Web.Enums;

namespace ReleaseBoard.Web.Extensions
{
    /// <summary>
    /// Расширения для <see cref="ClaimsPrincipal"/>.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Является ли текущий пользователь админом.
        /// </summary>
        /// <param name="user">Текущий пользователь.</param>
        /// <returns>Является ли админом.</returns>
        public static bool IsAdmin(this ClaimsPrincipal user) =>
            user.IsInRole(Role.Admin.ToString("G"));
    }
}
