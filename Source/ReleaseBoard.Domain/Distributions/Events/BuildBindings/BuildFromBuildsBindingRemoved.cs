using System;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions.Events.BuildBindings
{
    /// <summary>
    /// События удаления сборок из привязки.
    /// </summary>
    public record BuildFromBuildsBindingRemoved(BuildsBinding BuildsBinding, Guid[] BuildIds, IMetadata Metadata) 
        : Event(Metadata);
}
