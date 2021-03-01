using System;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Builds.Events
{
    /// <summary>
    /// Изменение статуса.
    /// </summary>
    public record LifeCycleStateChanged(LifeCycleState LifeCycleState, IMetadata Metadata) 
        : Event(Metadata);
}
