using System;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Builds.Events
{
    /// <summary>
    /// Cобытия отметка как отслеживаемая сборки.
    /// </summary>
    public record BuildMarkAsTracked(string Location, Guid DistributionId, IMetadata Metadata) 
        : Event(Metadata);
}
