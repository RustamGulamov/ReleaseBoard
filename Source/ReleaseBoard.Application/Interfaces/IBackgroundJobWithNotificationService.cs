using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire.Server;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Application.Interfaces
{
    /// <summary>
    /// Интефейс сервиса фоновых задач с нотификацией.
    /// </summary>
    public interface IBackgroundJobWithNotificationService
    {
        /// <summary>
        /// Парсить билдов.
        /// </summary>
        /// <param name="buildBindings">Список привязок к билдам.</param>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="context"><see cref="PerformContext"/>.</param>
        /// <returns>Список парсированных задач.</returns>
        Task<IEnumerable<BuildDto>> ParseBuilds(BuildsBinding[] buildBindings, string userId, PerformContext context);
    }
}
