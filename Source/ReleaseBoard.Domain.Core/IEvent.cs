using System;
using MediatR;

namespace ReleaseBoard.Domain.Core
{
    /// <summary>
    /// Интерфейс домена событий.
    /// </summary>
    public interface IEvent : INotification
    {
    }
}
