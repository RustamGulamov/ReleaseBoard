using System;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Builds.Events
{
    /// <summary>
    /// События "сборка помечена как удаленная".
    /// </summary>
    public record BuildMarkAsUnTracked(Guid DistributionId, IMetadata Metadata) : Event(Metadata);
}
