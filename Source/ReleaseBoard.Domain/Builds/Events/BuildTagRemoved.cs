using System;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Builds.Events
{
    /// <summary>
    /// События удаления тега.
    /// </summary>
    public record BuildTagRemoved(string Tag, IMetadata Metadata) : Event(Metadata);
}
