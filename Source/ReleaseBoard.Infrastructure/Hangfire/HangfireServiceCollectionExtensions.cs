using System;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using ReleaseBoard.Application.Interfaces;

namespace ReleaseBoard.Infrastructure.Hangfire
{
    /// <summary>
    ///  Extension методы <see cref="IServiceCollection"/>.
    /// </summary>
    internal static class HangfireServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрирует сервисы для Hangfire.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        /// <param name="configuration"><see cref="IConfiguration"/>.</param>
        public static void RegisterHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJobStorageService, JobStorageService>();
            services.AddScoped<IBackgroundJobWithNotificationService, BackgroundJobWithNotificationService>();

            services.AddHangfire(c =>
            {
                string connectionString = configuration.GetConnectionString("Hangfire");
                MongoUrl mongoUrl = MongoUrl.Create(connectionString);
                c.UseMongoStorage(
                    MongoClientSettings.FromUrl(mongoUrl),
                    mongoUrl.DatabaseName,
                    new MongoStorageOptions
                    {
                        MigrationOptions = new MongoMigrationOptions(MongoMigrationStrategy.Migrate),
                        Prefix = "background"
                    });

                c.UseFilter(new AutomaticRetryAttribute { Attempts = 10 });

                c.UseSerializerSettings(new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full
                });
            });

            services.AddHangfireServer();
        }
    }
}
