using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions.Events.Owners
{
    /// <summary>
    /// Событие о добавлении списка новых ответственных дистрибутива.
    /// </summary>
    public record DistributionOwnersAdded(IList<User> Owners, IMetadata Metadata) : Event(Metadata);
}
