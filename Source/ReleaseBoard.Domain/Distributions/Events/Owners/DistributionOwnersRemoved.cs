using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions.Events.Owners
{
    /// <summary>
    /// Событие об удалении списка ответственных из дистрибутива.
    /// </summary>
    public record DistributionOwnersRemoved(IList<User> Owners, IMetadata Metadata) : Event(Metadata);
}
