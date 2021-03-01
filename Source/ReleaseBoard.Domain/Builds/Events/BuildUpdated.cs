using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Builds.Events
{
    /// <summary>
    /// События сборка обновлена.
    /// </summary>
    public record BuildUpdated(
            string Location, 
            VersionNumber Number, 
            VersionNumber ReleaseNumber, 
            IList<string> Suffixes, 
            IMetadata Metadata) 
        : Event(Metadata);
}
