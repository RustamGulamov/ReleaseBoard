using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Sentry;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ReleaseBoard.Web
{
    /// <summary>
    /// Входная точка запуска веб сервиса.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Собирает хост веб приложения.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        /// <returns>Подготовленный строитель веб приложения.</returns>
        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder
                        .UseIISIntegration()
                        .ConfigureAppConfiguration((hostingContext, config) =>
                        {
                            var env = hostingContext.HostingEnvironment;
                            config
                                .AddJsonFile("appsettings.json", false, true)
                                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                                .AddJsonFile("/config/appsettings.json", true, true)
                                .AddEnvironmentVariables("ReleaseBoard_Backend_");
                        })
                        .UseStartup<Startup>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(context.Configuration);
                    logging.AddSentry(x =>
                    {
                        x.BeforeSend = @event =>
                        {
                            @event.SetTag("Application", "Backend");
                            return @event;
                        };
                    });
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
        }

        /// <summary>
        /// Входная точка программы.
        /// </summary>
        /// <param name="args">Аргументы командной строки.</param>
        /// <returns><see cref="Task"/>.</returns>
        public static async Task Main(string[] args)
        {
            var logger = NLogBuilder
                .ConfigureNLog("nlog.config")
                .GetCurrentClassLogger();

            try
            {
                logger.Debug("Starting up webhost.");
                await CreateWebHostBuilder(args)
                    .Build()
                    .RunAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped service due to exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}

