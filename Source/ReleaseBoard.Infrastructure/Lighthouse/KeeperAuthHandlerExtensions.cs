using System;
using System.Net.Http;
using Lighthouse.Contracts.HttpClientHandlers;
using Lighthouse.Contracts.Providers;
using Lighthouse.Contracts.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ReleaseBoard.Infrastructure.Lighthouse
{
    /// <summary>
    ///  Extension методы <see cref="IServiceProvider"/>.
    /// </summary>
    public static class KeeperAuthHandlerExtensions
    {
        /// <summary>
        /// Создать обработчик авторизации.
        /// </summary>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/>.</param>
        /// <param name="keeperAuthSettings"><see cref="KeeperAuthSettings"/>.</param>
        /// <returns>.</returns>
        public static DelegatingHandler CreateKeeperAuthHandler(this IServiceProvider serviceProvider, KeeperAuthSettings keeperAuthSettings)
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var logger = serviceProvider.GetRequiredService<ILogger<KeeperAccessTokenProvider>>();
            var keeperAccessTokenProvider = new KeeperAccessTokenProvider(keeperAuthSettings, httpClientFactory, logger);
            return new BearerAuthenticationHandler(keeperAccessTokenProvider);
        }
    }
}
