using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions.Events.BuildBindings
{
    /// <summary>
    /// Событие удаления привязок к сборкам.
    /// </summary>
    public record BuildBindingsRemoved(Dictionary<BuildsBinding, Guid[]> BindingsToBuilds, IMetadata Metadata) 
        : Event(Metadata);
}
