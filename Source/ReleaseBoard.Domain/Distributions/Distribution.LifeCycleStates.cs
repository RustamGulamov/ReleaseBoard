using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Common.Contracts.Extensions;
using ReleaseBoard.Domain.Builds;
using ReleaseBoard.Domain.Distributions.Events.LifeCycleStates;

namespace ReleaseBoard.Domain.Distributions
{
    /// <summary>
    /// Дистрибутив. Методы изменения <see cref="AvailableLifeCycles"/> и <see cref="LifeCycleStateRules"/>.
    /// </summary>
    public partial class Distribution
    {
        /// <summary>
        /// Добавляет новые доступные состояния сборок у дистрибутива.
        /// </summary>
        /// <param name="lifeCycleStates">Новые доступные состояния сборок у дистрибутива.</param>
        public void AddAvailableLifeCycleStates(IEnumerable<LifeCycleState> lifeCycleStates)
        {
            IEnumerable<LifeCycleState> newLifeCycleStates = lifeCycleStates.Except(AvailableLifeCycles);
            if (newLifeCycleStates.IsEmpty())
            {
                return;
            }

            Apply(new DistributionAvailableLifeCyclesAdded(lifeCycleStates.ToArray(), CreateMetadata()));
        }

        /// <summary>
        /// Удаляет состояния сборок из дистрибутива.
        /// </summary>
        /// <param name="lifeCycleStates">Список состояния сборок для удаления из дистрибутива.</param>
        public void RemoveAvailableLifeCycleStates(IEnumerable<LifeCycleState> lifeCycleStates)
        {
            IEnumerable<LifeCycleState> lifeCycleStatesToRemove = lifeCycleStates.Intersect(AvailableLifeCycles);
            if (lifeCycleStatesToRemove.IsEmpty())
            {
                return;
            }

            Apply(new DistributionAvailableLifeCyclesRemoved(lifeCycleStates.ToArray(), CreateMetadata()));
        }

        /// <summary>
        /// Добавляет новые имена правил перехода по состояниям сборки.
        /// </summary>
        /// <param name="rules">Новые имена правил перехода по состояниям сборки.</param>
        public void AddLifeCycleStateRules(IEnumerable<string> rules)
        {
            IEnumerable<string> newRules = rules.Except(LifeCycleStateRules, StringComparer.OrdinalIgnoreCase);
            if (newRules.IsEmpty())
            {
                return;
            }

            Apply(new DistributionLifeCycleStateRulesAdded(newRules.ToArray(), CreateMetadata()));
        }

        /// <summary>
        /// Удаляет имена правил перехода по состояниям сборки.
        /// </summary>
        /// <param name="rules">Имена правил перехода для удаления.</param>
        public void RemoveLifeCycleStateRules(IEnumerable<string> rules)
        {
            IEnumerable<string> rulesToDelete = rules.Intersect(LifeCycleStateRules, StringComparer.OrdinalIgnoreCase);
            if (rulesToDelete.IsEmpty())
            {
                return;
            }

            Apply(new DistributionLifeCycleStateRulesRemoved(rulesToDelete.ToArray(), CreateMetadata()));
        }

        private void When(DistributionAvailableLifeCyclesAdded @event)
        {
            AvailableLifeCycles.AddRange(@event.AvailableLifeCycles);
        }

        private void When(DistributionAvailableLifeCyclesRemoved @event)
        {
            AvailableLifeCycles.RemoveAll(@event.AvailableLifeCycles.Contains);
        }

        private void When(DistributionLifeCycleStateRulesAdded @event)
        {
            LifeCycleStateRules.AddRange(@event.LifeCycleStateRules);
        }

        private void When(DistributionLifeCycleStateRulesRemoved @event)
        {
            LifeCycleStateRules.RemoveAll(@event.LifeCycleStateRules.Contains);
        }
    }
}
