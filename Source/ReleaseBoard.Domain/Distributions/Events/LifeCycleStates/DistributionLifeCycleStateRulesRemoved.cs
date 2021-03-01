using System;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Distributions.Events.LifeCycleStates
{
    /// <summary>
    /// События удаления имен правил перехода по состояниям сборки.
    /// </summary>
    public record DistributionLifeCycleStateRulesRemoved(string[] LifeCycleStateRules, IMetadata Metadata)
        : Event(Metadata);
}
