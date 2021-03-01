using System;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Distributions.Events.LifeCycleStates
{
    /// <summary>
    /// События удаления доступных состояний.
    /// </summary>
    public record DistributionAvailableLifeCyclesRemoved(LifeCycleState[] AvailableLifeCycles, IMetadata Metadata) 
        : Event(Metadata);
}
