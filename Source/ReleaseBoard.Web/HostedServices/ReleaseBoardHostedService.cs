using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReleaseBoard.Web.HostedServices.StartupTasks;

namespace ReleaseBoard.Web.HostedServices
{
    /// <summary>
    /// Реализует методы сервиса.
    /// </summary>
    public class ReleaseBoardHostedService : IHostedService
    {
        private readonly ILogger<ReleaseBoardHostedService> logger;
        private readonly IHandlersRegistrator registrator;
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="logger">Логгер <see cref="ILogger"/>.</param>
        /// <param name="registrator">Регистратор.</param>
        /// <param name="serviceProvider"><see cref="IServiceProvider"/>.</param>
        public ReleaseBoardHostedService(
            ILogger<ReleaseBoardHostedService> logger, IHandlersRegistrator registrator,
            IServiceProvider serviceProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.registrator = registrator ?? throw new ArgumentNullException(nameof(registrator));
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting service!");
            
            registrator.RegisterAll();

            await ExecuteStartTasks();
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping service!");
            return Task.CompletedTask;
        }

        private async Task ExecuteStartTasks()
        {
            using var scope = serviceProvider.CreateScope();
            var startupTasks = 
                scope
                    .ServiceProvider
                    .GetServices<IStartupTask>()
                    .Select(x => x.Execute());

            await Task.WhenAll(startupTasks);
        }
    }
}
