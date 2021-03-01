using System;
using System.Threading.Tasks;

namespace ReleaseBoard.Web.SignalR
{
    /// <summary>
    /// Интерфейс хаба нотификации.
    /// </summary>
    public interface INotificationHub
    {
        /// <summary>
        /// Публиковать события для выполнимых задач.
        /// </summary>
        /// <param name="limit">Лимит.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task PublishJobEventsForProcessingJobs(int limit);
    }
}
