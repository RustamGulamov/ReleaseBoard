using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Common.Contracts.Extensions;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Extensions;
using ReleaseBoard.Domain.Distributions.Events;
using ReleaseBoard.Domain.Distributions.Exceptions;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions
{
    /// <summary>
    /// Дистрибутив.
    /// </summary>
    public partial class Distribution : Aggregate
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="events">События.</param>
        public Distribution(IEnumerable<Event> events)
        {
            Replay(events);
        }

        /// <summary>
        /// Конструктор для создания нового дистрибутива.
        /// </summary>
        /// <param name="name">Имя.</param>
        /// <param name="owners">Ответственные.</param>
        /// <param name="availableLifeCycles">Список доступных состояний сборок у дистрибутива.</param>
        /// <param name="lifeCycleStateRules">Имена правил перехода по состояниям сборки.</param>
        public Distribution(string name, IEnumerable<User> owners, IEnumerable<LifeCycleState> availableLifeCycles, IEnumerable<string> lifeCycleStateRules)
        {
            name.ThrowIsNullOrWhiteSpace(new CreateDistributionException($"{nameof(Name)} is null."));
            ValidateCollection(owners);
            ValidateCollection(availableLifeCycles);
            ValidateCollection(lifeCycleStateRules);
            
            Id = Guid.NewGuid();
            
            Apply(
                new DistributionCreated(
                    Id, 
                    name, 
                    owners.ToArray(),
                    availableLifeCycles.ToArray(), 
                    lifeCycleStateRules.ToArray(),
                    CreateMetadata())
                );
        }
        
        /// <summary>
        /// Имя дистрибутива.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Ответственные.
        /// </summary>
        public List<User> Owners { get; } = new();

        /// <summary>
        /// Список доступных состояний сборок у дистрибутива.
        /// </summary>
        public List<LifeCycleState> AvailableLifeCycles { get; } = new();

        /// <summary>
        /// Имена правил перехода по состояниям сборки.
        /// </summary>
        public List<string> LifeCycleStateRules { get; } = new();

        /// <summary>
        /// Cвязь между привязками и сборками.
        /// </summary>
        public Dictionary<BuildsBinding, List<Guid>> BindingToBuilds { get; } = new();

        /// <summary>
        /// Привязки к проектам.
        /// </summary>
        public List<ProjectBinding> ProjectBindings { get; } = new();

        /// <inheritdoc />
        public override string ToString() => Name;

        /// <summary>
        /// Обновляет имя дистрибутива.
        /// </summary>
        /// <param name="name">Имя.</param>
        public void UpdateName(string name)
        {
            name.ThrowIsNullOrWhiteSpace(new UpdateDistributionException("Name is invalid"));

            if (name.Equals(Name))
            {
                return;
            }

            Apply(new DistributionNameUpdated(name, CreateMetadata()));
        }

        /// <inheritdoc/>
        protected override void Mutate(Event @event)
        {
            ((dynamic)this).When((dynamic)@event);
        }

        private void ValidateCollection<T>(IEnumerable<T> collection)
        {
            collection.ThrowIfNull(new CreateDistributionException($"{nameof(collection)} is null."));
            
            if (collection.IsEmpty())
            {
                throw new CreateDistributionException($"{nameof(collection)} cannot be empty.");
            }
        }

        private void When(DistributionCreated @event)
        {
            (Guid guid, string name, User[] owners, LifeCycleState[] availableLifeCycles, string [] lifeCycleStateRules, _) = @event;
            Id = guid;
            Name = name;
            Owners.AddRange(owners);
            AvailableLifeCycles.AddRange(availableLifeCycles);
            LifeCycleStateRules.AddRange(lifeCycleStateRules);
        }

        private void When(DistributionNameUpdated @event)
        {
            Name = @event.Name;
        }
    }
}
