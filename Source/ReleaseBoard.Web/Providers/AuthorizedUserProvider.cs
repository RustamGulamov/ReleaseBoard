using System;
using Microsoft.AspNetCore.Http;
using ReleaseBoard.Web.ApiModels;

namespace ReleaseBoard.Web.Providers
{
    /// <summary>
    /// Провайдер для доступа к текущему пользователю.
    /// </summary>
    public class AuthorizedUserProvider : IAuthorizedUserProvider
    {
        private readonly HttpContext httpContext;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="httpContextAccessor"><see cref="HttpContextAccessor"/>.</param>
        public AuthorizedUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            httpContextAccessor.ThrowIfNull(nameof(httpContextAccessor));

            httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <inheritdoc />
        public AuthorizedUser Get()
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return AuthorizedUser.Anonymous;
            }

            return AuthorizedUser.CreateFromClaimPrincipal(httpContext.User);
        }
    }
}
