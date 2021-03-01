using System;
using System.Threading.Tasks;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Application.Interfaces
{
    /// <summary>
    /// Сервис взаимодействия с Lighthouse.
    /// </summary>
    public interface ILighthouseServiceApi
    {
        /// <summary>
        /// Поиск сотрудника по идентификатору.
        /// </summary>
        /// <param name="sid">Идентификатор сотрудника sid.</param>
        /// <returns>Сотрудник.</returns>
        Task<User> GetUserBySid(string sid);

        /// <summary>
        /// Являются ли пользователи валидными.
        /// </summary>
        /// <param name="sids">Идентификаторы сотрудников sid.</param>
        /// <returns>True - валидный.</returns>
        Task<bool> IsValidUsers(params string[] sids);

        /// <summary>
        /// Получить проект по идентификатору.
        /// </summary>
        /// <param name="externalId">Внешний идентификатор проекта.</param>
        /// <returns>Проект.</returns>
        Task<Project> GetProjectById(Guid externalId);
    }
}
