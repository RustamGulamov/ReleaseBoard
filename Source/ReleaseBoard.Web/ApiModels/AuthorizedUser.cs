using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityModel;
using ReleaseBoard.Web.Enums;

namespace ReleaseBoard.Web.ApiModels
{
    /// <summary>
    /// Авторизованный пользователь.
    /// </summary>
    public class AuthorizedUser
    {
        private HashSet<string> roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="id">Идентификатор пользователя.</param>
        /// <param name="name">Имя пользователя.</param>
        /// <param name="email">Имеил пользователя.</param>
        /// <param name="sid">Имеил пользователя.</param>
        public AuthorizedUser(string id, string name, string email, string sid)
        {
            Id = id;
            Name = name;
            Email = email;
            Sid = sid;
        }

        /// <summary>
        /// Неавторизованный пользователь.
        /// </summary>
        public static AuthorizedUser Anonymous => new AuthorizedUser(string.Empty, "Anonymous", string.Empty, string.Empty);

        /// <summary>
        /// Имеил пользователя.
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Роли пользователя.
        /// </summary>
        public IReadOnlyCollection<string> Roles => roles;

        /// <summary>
        /// Является ли пользователь админом.
        /// </summary>
        public bool IsAdmin => Roles.Contains(Role.Admin.ToString());

        /// <summary>
        /// Идентификатор AD.
        /// </summary>
        public string Sid { get; }

        /// <summary>
        /// Создает роль из <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="claimsPrincipal">Приципал.</param>
        /// <returns><see cref="AuthorizedUser"/>.</returns>
        public static AuthorizedUser CreateFromClaimPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            string id = claimsPrincipal.FindFirst("Id").Value;
            string name = claimsPrincipal.FindFirst(ClaimTypes.Name).Value;
            string email = claimsPrincipal.FindFirst(JwtClaimTypes.Email).Value;
            string sid = claimsPrincipal.FindFirst(ClaimTypes.Sid).Value;
            AuthorizedUser user = new AuthorizedUser(id, name, email, sid);

            List<string> roles = claimsPrincipal
                .FindAll(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value)
                .ToList();

            if (roles.Any())
            {
                user.AddRoles(roles);
            }

            return user;
        }

        /// <inheritdoc/>
        public override string ToString() => Name;

        /// <summary>
        /// Добавляет роли.
        /// </summary>
        /// <param name="roles">Роли.</param>
        private void AddRoles(List<string> roles)
        {
            this.roles.UnionWith(roles);
        }
    }
}
