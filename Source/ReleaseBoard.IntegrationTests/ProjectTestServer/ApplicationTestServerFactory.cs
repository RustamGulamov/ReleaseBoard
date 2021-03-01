using System;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.IntegrationTests.ProjectTestServer.Services;
using ReleaseBoard.Web;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer
{
    /// <summary>
    /// Фабрика для создания тестового сервера для проекта <see cref="ReleaseBoard"/>.
    /// </summary>
    public class ApplicationTestServerFactory : TestServerFixture<Startup>
    {
        /// <inheritdoc />
        protected override void ConfigureMockServices(IServiceCollection services)
        {
            services.AddScoped<IBackgroundJobClient>(_ => MockedServices.BackgroundJobClientMock.Object);

            services.AddScoped<IJobStorageService>(_ =>
            {
                var mock = new Mock<IJobStorageService>();
                mock.Setup(x => x.SetJobParameter(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

                return mock.Object;
            });
        }
    }
}
