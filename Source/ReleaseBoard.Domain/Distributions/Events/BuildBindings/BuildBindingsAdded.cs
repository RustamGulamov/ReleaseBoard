using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions.Events.BuildBindings
{
    /// <summary>
    /// Событие добавления привязок к сборкам.
    /// </summary>
    public record BuildBindingsAdded(Dictionary<BuildsBinding, Guid[]> BindingToBuilds, IMetadata Metadata) 
        : Event(Metadata);
}
