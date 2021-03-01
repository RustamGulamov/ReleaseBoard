using System;
using System.Collections.Generic;

namespace ReleaseBoard.Application.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для работы Job из Hangfire.
    /// </summary>
    public interface IJobStorageService
    {
        /// <summary>
        /// Получить все активные фоновые задачи.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <param name="limit">Лимит.</param>
        /// <returns>Список пользователей и активные фоновые задачи.</returns>
        List<(string, string)> GetAllActiveJobsForUser(string userId, int limit = int.MaxValue);

        /// <summary>
        /// Задать параметры для Job.
        /// </summary>
        /// <param name="jobId">Идентификатор Job.</param>
        /// <param name="eventType">Тип события.</param>
        /// <param name="userId">Идентификатор ползователя.</param>
        void SetJobParameter(string jobId, string eventType, string userId);

        /// <summary>
        /// Получить результат фоновой задачи.
        /// </summary>
        /// <param name="jobId">Идентификатор задачи.</param>
        /// <returns>Результат.</returns>
        T GetJobResult<T>(string jobId) where T : class;
    }
}
