using System;
using ReleaseBoard.Common.Contracts.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace ReleaseBoard.Infrastructure.StaticStorage
{
    /// <summary>
    /// Фабрика клиента для доступа к статическому хранилищу.
    /// </summary>
    public class StaticStorageClientFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly StaticStorageUrlProvider staticStorageUrlProvider;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="serviceProvider">DI контейнер.</param>
        /// <param name="staticStorageUrlProvider">Поставщик адресов для статического хранилища.</param>
        public StaticStorageClientFactory(IServiceProvider serviceProvider, StaticStorageUrlProvider staticStorageUrlProvider)
        {
            this.serviceProvider = serviceProvider;
            this.staticStorageUrlProvider = staticStorageUrlProvider;
        }

        /// <summary>
        /// Получить клиент.
        /// </summary>
        /// <param name="buildSourceType">Тип сборки.</param>
        /// <returns>Клиент StaticService.</returns>
        public StaticStorageClient Get(BuildSourceType buildSourceType)
        {
            var url = staticStorageUrlProvider.GetServiceUrl(buildSourceType);

            var client = serviceProvider.GetService<StaticStorageClient>();
            client.ServiceUrl = url;

            return client;
        }
    }
}
