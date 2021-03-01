using System;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions.Events
{
    /// <summary>
    /// Создание дистрибутива.
    /// </summary>
    public record DistributionCreated(
            Guid Id, 
            string Name, 
            User[] Owners, 
            LifeCycleState[] AvailableLifeCycles,
            string[] LifeCycleStateRules,
            IMetadata Metadata) 
        : Event(Metadata);
}
