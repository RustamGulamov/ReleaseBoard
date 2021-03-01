using System;
using ReleaseBoard.Domain.SignalREvents.Abstractions;

namespace ReleaseBoard.Domain.SignalREvents.Server
{
    /// <summary>
    /// События сервера.
    /// </summary>
    public record ServerSignalREvent(string Type, object Payload, string[] RecipientsUserIds) 
        : SignalREvent($"@event/{Type}", Payload);
}
