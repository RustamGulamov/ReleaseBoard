using System;
using ReleaseBoard.Common.Contracts.Abstractions;

namespace ReleaseBoard.Infrastructure.StaticStorage
{
    /// <summary>
    /// Поставщик адресов для StaticStorage.
    /// </summary>
    public class StaticStorageUrlProvider
    {
        private readonly StaticStorageSettings settings;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="settings"><see cref="StaticStorageSettings"/>.</param>
        public StaticStorageUrlProvider(StaticStorageSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Получить адрес StaticStorage.
        /// </summary>
        /// <param name="buildSourceType">Тип хранилища сборки.</param>
        /// <returns>Адрес StaticStorage.</returns>
        public string GetServiceUrl(BuildSourceType buildSourceType)
        {
            string result = settings.Services.TryGetValue(buildSourceType, out string baseUrl)
                ? baseUrl
                : throw new NotSupportedException($"Не удалось найти путь StaticStorage сервиса для {buildSourceType:G}");

            return result;
        }
    }
}
