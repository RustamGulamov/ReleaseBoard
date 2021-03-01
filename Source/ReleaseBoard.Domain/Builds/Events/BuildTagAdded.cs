using System;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Domain.Builds.Events
{
    /// <summary>
    /// События добавления тега.
    /// </summary>
    public record BuildTagAdded(string Tag, IMetadata Metadata) : Event(Metadata);
}
