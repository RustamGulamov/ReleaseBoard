using System;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions.Events.ProjectBindings
{
    /// <summary>
    /// Обновление привязок к проектам.
    /// </summary>
    public record ProjectBindingRemoved(ProjectBinding ProjectBinding, IMetadata Metadata) : Event(Metadata);
}
