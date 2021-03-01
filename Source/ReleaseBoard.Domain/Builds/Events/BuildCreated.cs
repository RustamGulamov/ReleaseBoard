using System;
using System.Collections.Generic;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Builds.Events
{
    /// <summary>
    /// Создана сборка.
    /// </summary>
    public record BuildCreated(
        Guid Id,
        DateTime BuildDate,
        Guid DistributionId,
        string Location,
        VersionNumber Number,
        VersionNumber ReleaseNumber,
        LifeCycleState LifeCycleState,
        BuildSourceType SourceType,
        IList<string> Suffixes,
        IMetadata Metadata
    ) : Event(Metadata);
}
