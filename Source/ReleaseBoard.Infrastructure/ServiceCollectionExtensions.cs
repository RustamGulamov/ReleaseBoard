using System;
using Lighthouse.Contracts.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Infrastructure.BuildSync;
using ReleaseBoard.Infrastructure.Data.MongoDb;
using ReleaseBoard.Infrastructure.Hangfire;
using ReleaseBoard.Infrastructure.Lighthouse;
using ReleaseBoard.Infrastructure.StaticStorage;

namespace ReleaseBoard.Infrastructure
{
    /// <summary>
    /// Extension методы <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрирует конфиги инфроструктуры.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        /// <param name="configuration">><see cref="IConfiguration"/>.</param>
        public static void RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterMongoDB(configuration.GetConnectionString("ReleaseBoard"));
            services.RegisterBuildSync(configuration);
            services.RegisterRawRabbit(configuration);
            services.RegisterHangfire(configuration);
            services.RegisterStaticStorageService(configuration);
            services.AddSingleton<ILighthouseServiceApi, LighthouseServiceApi>();
            services.AddLighthouseClients(configuration);
        }
    }
}
