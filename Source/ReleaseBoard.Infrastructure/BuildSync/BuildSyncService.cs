using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Common.Contracts.BuildSync.Requests;
using ReleaseBoard.Common.Contracts.BuildSync.Responses;
using ReleaseBoard.Common.Infrastructure.Common.Constants;
using ReleaseBoard.Common.Infrastructure.Rabbit.Extensions;
using ReleaseBoard.Common.Infrastructure.Rabbit.Options;
using RawRabbit;
using ReleaseBoard.Application.Interfaces;

namespace ReleaseBoard.Infrastructure.BuildSync
{
    /// <summary>
    /// Сервис для взаимодействия с BuildSync.
    /// </summary>
    public class BuildSyncService : IBuildSyncService
    {
        private readonly IBusClient busClient;
        private readonly SendRequestOptions sendRequestOptions;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="busClient">Клиент rabbit.</param>
        /// <param name="rabbitSettings">Настройки rabbit.</param>
        public BuildSyncService(IBusClient busClient, ReleaseBoardRabbitSettings rabbitSettings)
        {
            this.busClient = busClient;
            sendRequestOptions = new SendRequestOptions
            {
                RequestExchange = rabbitSettings.ReleaseBoardBuildSyncExchange,
                ResponseExchange = rabbitSettings.ReleaseBoardBuildSyncExchange,
                ResponseQueue = rabbitSettings.BuildSyncResponseQueue,
                RoutingKey = RoutingKeys.ReleaseBoardRequest,
            };
        }

        /// <summary>
        /// Отправляет команду на изменение суффиксов у сборки.
        /// </summary>
        /// <param name="buildLocation">Путь к сборке.</param>
        /// <param name="sourceType">Тип источника.</param>
        /// <param name="suffixes">Суффиксы для записи.</param>
        /// <param name="currentSuffixes">Нынешние суффиксы сборки.</param>
        /// <returns><see cref="Task"/>.</returns>
        public async Task<IResponseMessage> ChangeSuffix(string buildLocation, BuildSourceType sourceType, List<string> suffixes, List<string> currentSuffixes)
        {
            var request = new ChangeSuffixesRequest { BuildPath = buildLocation, SourceType = sourceType, Suffixes = suffixes, CurrentSuffixes = currentSuffixes };

            return await busClient.SendRequest<ChangeSuffixesRequest, IResponseMessage>(request, sendRequestOptions);
        }

        /// <summary>
        /// Поиск сборок.
        /// </summary>
        /// <param name="path">Путь к каталогу для поиска.</param>
        /// <param name="sourceType">Тип источника сборок.</param>
        /// <param name="pattern">Список патернов для распознавания сборок.</param>
        /// <returns>Результат поиска.</returns>
        public async Task<ParseBuildsResponse> ParseBuilds(string path, BuildSourceType sourceType, string pattern)
        {
            var request = new ParseBuildsRequest
            {
                Path = path,
                Pattern = pattern,
                SourceType = sourceType
            };
            return await busClient.SendRequest<ParseBuildsRequest, ParseBuildsResponse>(request, sendRequestOptions);
        }

        /// <summary>
        /// Поиск сборок.
        /// </summary>
        /// <param name="path">Путь к каталогу для поиска.</param>
        /// <param name="sourceType">Тип источника сборок.</param>
        /// <param name="patternsToCheck">Список патернов для распознавания сборок.</param>
        /// <returns>Результат поиска.</returns>
        public async Task<IResponseMessage> SearchBuilds(string path, BuildSourceType sourceType, IEnumerable<string> patternsToCheck)
        {
            var request = new ScanForBuildsRequest
            {
                Path = path,
                PatternsToCheck = patternsToCheck.ToList(),
                SourceType = sourceType
            };

            return await busClient.SendRequest<ScanForBuildsRequest, IResponseMessage>(request, sendRequestOptions);
        }
    }
}
