using System;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Distributions.Events.LifeCycleStates
{
    /// <summary>
    /// События добавления новых имен правил перехода по состояниям сборки.
    /// </summary>
    public record DistributionLifeCycleStateRulesAdded(string[] LifeCycleStateRules, IMetadata Metadata) 
        : Event(Metadata);
}
