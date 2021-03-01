using System;

namespace ReleaseBoard.Domain.Core
{
    /// <summary>
    /// Доменное событие.
    /// </summary>
    public abstract record Event(IMetadata Metadata) : IEvent;
}
