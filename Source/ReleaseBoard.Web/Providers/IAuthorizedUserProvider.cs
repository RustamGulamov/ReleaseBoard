using System;
using ReleaseBoard.Web.ApiModels;

namespace ReleaseBoard.Web.Providers
{
    /// <summary>
    ///     Интерфейс для доступа к текущему пользователю.
    /// </summary>
    public interface IAuthorizedUserProvider
    {
        /// <summary>
        /// Возвращает объект текущего пользователя типа <see cref="AuthorizedUser"/>.
        /// </summary>
        /// <returns>Текущий пользователь.</returns>
        AuthorizedUser Get();
    }
}
