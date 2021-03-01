using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReleaseBoard.Application.Interfaces
{
    /// <summary>
    /// Интерфейс для взаимодействия с сервисом BuildSync.
    /// </summary>
    public interface IBuildSyncService
    {
        /// <summary>
        /// Отправляет команду на изменение суффиксов у сборки.
        /// </summary>
        /// <param name="buildLocation">Путь к сборке.</param>
        /// <param name="sourceType">Тип источника.</param>
        /// <param name="suffixes">Суффиксы для записи.</param>
        /// <param name="currentSuffixes">Нынешние суффиксы сборки.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task<IResponseMessage> ChangeSuffix(string buildLocation, BuildSourceType sourceType, List<string> suffixes, List<string> currentSuffixes);

        /// <summary>
        /// Поиск сборок.
        /// </summary>
        /// <param name="path">Путь к каталогу для поиска.</param>
        /// <param name="sourceType">Тип источника сборок.</param>
        /// <param name="pattern">Патерн для распознавания сборок.</param>
        /// <returns>Результат поиска.</returns>
        Task<ParseBuildsResponse> ParseBuilds(string path, BuildSourceType sourceType, string pattern);

        /// <summary>
        /// Поиск сборок.
        /// </summary>
        /// <param name="path">Путь к каталогу для поиска.</param>
        /// <param name="sourceType">Тип источника сборок.</param>
        /// <param name="patternsToCheck">Список патернов для распознавания сборок.</param>
        /// <returns>Результат поиска.</returns>
        Task<IResponseMessage> SearchBuilds(string path, BuildSourceType sourceType, IEnumerable<string> patternsToCheck);
    }
}