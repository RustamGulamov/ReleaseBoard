using System;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Builds.Events
{
    /// <summary>
    /// Изменение статус артефакта.
    /// </summary>
    public record ArtifactStateChanged(ArtifactState ArtifactState, IMetadata Metadata) 
        : Event(Metadata);
}
