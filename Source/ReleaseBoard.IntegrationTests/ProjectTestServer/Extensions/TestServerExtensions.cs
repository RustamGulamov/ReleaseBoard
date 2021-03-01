using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;

namespace ReleaseBoard.IntegrationTests.ProjectTestServer.Extensions
{
    /// <summary>
    /// Методы расширения для <see cref="TestServer"/>.
    /// </summary>
    internal static class TestServerExtensions
    {
        private static string accessToken = "";

        /// <summary>
        /// Подключиться к серверу для работы с SignalR по имени хаба.
        /// </summary>
        /// <param name="server"><see cref="TestServer"/>.</param>
        /// <param name="hubName">Название хаба.</param>
        /// <returns><see cref="Task{HubConnection}"/>.</returns>
        internal static async Task<HubConnection> StartHubConnectionAsync(this TestServer server, string hubName)
        {
            HttpClient client = server.CreateClient();
            HubConnection hubConnection =
                new HubConnectionBuilder()
                    .WithUrl($"{client.BaseAddress.AbsoluteUri}{hubName}", o =>
                    {
                        o.AccessTokenProvider = () => Task.FromResult(accessToken);
                        o.HttpMessageHandlerFactory = _ => server.CreateHandler();
                    })
                    .Build();

            await hubConnection.StartAsync();

            return hubConnection;
        }

        /// <summary>
        ///     Создать авторизованный пользователь.
        /// </summary>
        /// <param name="server"><see cref="TestServer"/>.</param>
        /// <returns><see cref="HttpClient"/>.</returns>
        internal static HttpClient CreateAuthClient(this TestServer server)
        {
            HttpClient client = server.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            return client;
        }
    }
}
