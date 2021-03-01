using System;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Distributions.Events.LifeCycleStates
{
    /// <summary>
    /// События добавления новых доступных состояний.
    /// </summary>
    public record DistributionAvailableLifeCyclesAdded(LifeCycleState[] AvailableLifeCycles, IMetadata Metadata) 
        : Event(Metadata);
}
