using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using ReleaseBoard.Infrastructure.Lighthouse;

namespace ReleaseBoard.Infrastructure.StaticStorage
{
    /// <summary>
    /// Extension методы <see cref="IServiceCollection"/>.
    /// </summary>
    public static class StaticStorageServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрация StaticStorageService.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">><see cref="IConfiguration"/>.</param>
        public static void RegisterStaticStorageService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.BindConfig<StaticStorageSettings>());
            var keeperAuthSettings = configuration.GetSection(nameof(KeeperAuthSettings)).Get<KeeperAuthSettings>();

            services.AddTransient<StaticStorageClient>();
            services.AddHttpClient<StaticStorageClient>()
                .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler { UseDefaultCredentials = true })
                .AddHttpMessageHandler(serviceProvider => serviceProvider.CreateKeeperAuthHandler(keeperAuthSettings))
                .UseHttpClientMetrics();

            services.AddTransient<StaticStorageUrlProvider>();
            services.AddTransient<StaticStorageClientFactory>();
            services.AddTransient<IStaticStorageFacade, StaticStorageFacade>();
        }
    }
}
