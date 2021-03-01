using System;
using System.Threading.Tasks;
using ReleaseBoard.Domain.SignalREvents.Abstractions;

namespace ReleaseBoard.Web.SignalR
{
    /// <summary>
    /// Интерфейс клиента SignalR.
    /// </summary>
    public interface ISignalRClient
    {
        /// <summary>
        /// Получить события.
        /// </summary>
        /// <param name="publicEvent"><see cref="SignalREvent"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task ReceiveEvent(SignalREvent publicEvent);
    }
}
