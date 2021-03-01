using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using ReleaseBoard.Domain.SignalREvents.Job;
using ReleaseBoard.IntegrationTests.ProjectTestServer;
using ReleaseBoard.IntegrationTests.ProjectTestServer.Extensions;
using ReleaseBoard.Web.SignalR;
using Xunit;

namespace ReleaseBoard.IntegrationTests.SignalR
{
    /// <summary>
    /// Тесты для <see cref="SignalREventHandler"/>.
    /// </summary>
    public class SignalREventHandlerTests : IClassFixture<ApplicationTestServerFactory>
    {
        private const string HubName = "notifications";
        private readonly IMediator mediator;
        private readonly TestServer server;
        private readonly JobSignalREvent fakeJobEvent =
            new JobSignalREvent("job name", new JobEventPayload("some id", 55), string.Empty);

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="factory"><see cref="ApplicationTestServerFactory"/>.</param>
        public SignalREventHandlerTests(ApplicationTestServerFactory factory)
        {
            server = factory.Server;
            mediator = server.Host.Services.GetService<IMediator>();
        }

        /// <summary>
        /// Тест проверяет обработку событий JobEvent и перехвата из SignalR.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task PublishEvent_JobEvent_EventWasTakenSuccessfullyFromSignalR()
        {
            JObject result = null;
            HubConnection connection = await server.StartHubConnectionAsync(HubName);
            connection.On<JObject>("ReceiveEvent", e =>
            {
                result = e;
            });

            await mediator.Publish(fakeJobEvent);
            await Task.Delay(2000);

            var type = result.GetValue(nameof(fakeJobEvent.Type), StringComparison.OrdinalIgnoreCase);
            Assert.Equal(fakeJobEvent.Type, type);
        }

        /// <summary>
        /// Тест для случая когда событие не обрабатывается.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        [Fact]
        public async Task PublishEvent_AnyEvent_ThrowsArgumentException()
        {
            object result = null;
            HubConnection connection = await server.StartHubConnectionAsync(HubName);
            connection.On<object>("ReceiveEvent", e =>
            {
                result = e;
            });

            await Assert.ThrowsAsync<ArgumentException>(() => mediator.Publish(new { Type = "any type" }));

            Assert.Null(result);
        }
    }
}
