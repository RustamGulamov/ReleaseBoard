using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer
{
    /// <summary>
    /// Базовый тестовый сервер предоставляет <see cref="TestServer"/> для интеграционных тестов.
    /// </summary>
    /// <typeparam name="TStartup">Тип класса конфигурации веб-сервера.</typeparam>
    public abstract class TestServerFixture<TStartup> : IDisposable
        where TStartup : class
    {
        private TestServer server;

        /// <summary>
        /// Тестовый сервер.
        /// </summary>
        public TestServer Server => server ?? (server = CreateServer());

        /// <summary>
        /// EnvironmentName.
        /// </summary>
        protected virtual string EnvironmentName => "Test";

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes object.
        /// </summary>
        /// <param name="disposing">Flag indicating whether managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Server?.Dispose();
            }
        }

        /// <summary>
        /// Создает тестовый сервер.
        /// </summary>
        /// <returns>Тестовый сервер.</returns>
        protected virtual TestServer CreateServer()
        {
            IWebHostBuilder builder = GetWebHostBuilder()
                .ConfigureTestServices(ConfigureMockServices);

            builder.ConfigureAppConfiguration((builderContext, config) =>
            {
                config.AddJsonFile("appsettings.json", false, true);
            });

            return new TestServer(builder);
        }

        /// <summary>
        /// Получить <see cref="WebHostBuilder"/>.
        /// </summary>
        /// <returns><see cref="WebHostBuilder"/>.</returns>
        protected virtual IWebHostBuilder GetWebHostBuilder() => 
            new WebHostBuilder()
                .UseStartup<TStartup>()
                .UseEnvironment(EnvironmentName);

        /// <summary>
        /// Переопределяет существующие сервисы в тестовом сервере.
        /// Используется для добавления фейковых реализаций сервисов.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        protected abstract void ConfigureMockServices(IServiceCollection services);
    }
}