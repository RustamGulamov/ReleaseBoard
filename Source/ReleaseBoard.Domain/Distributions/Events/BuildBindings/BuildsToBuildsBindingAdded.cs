using System;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions.Events.BuildBindings
{
    /// <summary>
    /// События добавления сборок в привязку.
    /// </summary>
    public record BuildsToBuildsBindingAdded(BuildsBinding BuildsBinding, Guid[] BuildIds, IMetadata Metadata) 
        : Event(Metadata);
}
