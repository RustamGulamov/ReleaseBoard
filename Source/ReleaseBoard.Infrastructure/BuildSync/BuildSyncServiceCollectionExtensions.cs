using System;
using System.Linq;
using ReleaseBoard.Common.Infrastructure.Common.Extensions;
using ReleaseBoard.Common.Infrastructure.Rabbit;
using ReleaseBoard.Common.Infrastructure.Rabbit.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Enrichers.GlobalExecutionId;
using RawRabbit.Instantiation;
using RawRabbit.Serialization;
using ReleaseBoard.Application.Interfaces;

namespace ReleaseBoard.Infrastructure.BuildSync
{
    /// <summary>
    /// Extension методы <see cref="IServiceCollection"/> для BuildSync.
    /// </summary>
    public static class BuildSyncServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрирует конфиги Монго БД.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        /// <param name="configuration"><see cref="IConfiguration"/>.</param>
        internal static void RegisterBuildSync(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IBuildSyncService, BuildSyncService>();
            services.AddSingleton<IEventMessageConsumer, BuildSyncEventsConsumer>();
        }

        /// <summary>
        /// Регистрирует конфиги Монго БД.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        /// <param name="configuration"><see cref="IConfiguration"/>.</param>
        internal static void RegisterRawRabbit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(x => configuration.BindConfig<ReleaseBoardRabbitSettings>());
            services.AddRawRabbit(new RawRabbitOptions
            {
                ClientConfiguration = GetRawRabbitConfiguration(configuration),
                Plugins = p => p.UseGlobalExecutionId(),
                DependencyInjection = ioc => ioc.AddSingleton<ISerializer>(new JsonSerializer(new RabbitJsonSerializer()))
            });
        }

        private static RawRabbitConfiguration GetRawRabbitConfiguration(IConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("RawRabbit");
            if (!section.GetChildren().Any())
            {
                throw new ArgumentException("Unable to configuration section 'RawRabbit'. Make sure it exists in the provided configuration");
            }

            return section.Get<RawRabbitConfiguration>();
        }
    }
}
