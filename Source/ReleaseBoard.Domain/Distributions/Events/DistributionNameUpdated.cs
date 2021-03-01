using System;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Distributions.Events
{
    /// <summary>
    /// Событие обновление имени.
    /// </summary>
    public record DistributionNameUpdated(string Name, IMetadata Metadata) : Event(Metadata);
}
