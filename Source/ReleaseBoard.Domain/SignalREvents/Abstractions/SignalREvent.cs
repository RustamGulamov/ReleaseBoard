using System;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.SignalREvents.Abstractions
{
    /// <summary>
    /// Интерфейс событий SignalR.
    /// </summary>
    public abstract record SignalREvent(string Type, object Payload) : IEvent;
}
