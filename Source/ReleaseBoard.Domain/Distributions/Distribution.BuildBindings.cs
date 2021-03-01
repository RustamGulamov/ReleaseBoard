using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Common.Contracts.Extensions;
using ReleaseBoard.Domain.Distributions.Events.BuildBindings;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions
{
    /// <summary>
    /// Дистрибутив. Методы изменения состояния привязки к билдам.
    /// </summary>
    public partial class Distribution
    {
        /// <summary>
        /// Добавляет новые привязки.
        /// </summary>
        /// <param name="buildBindings">Новые привязки к билдам для добавления.</param>
        /// <param name="actionUserId">Идентификатор пользователя, кто добавил привязки.</param>
        public void AddBuildBindings(IEnumerable<BuildsBinding> buildBindings, string actionUserId)
        {
            IEnumerable<BuildsBinding> newBuildBindings = buildBindings.Except(BindingToBuilds.Keys);
            if (newBuildBindings.IsEmpty())
            {
                return;
            }

            Dictionary<BuildsBinding, Guid[]> bindingsWithEmptyBuilds = 
                newBuildBindings
                    .ToDictionary(x => x, x => Array.Empty<Guid>());
            
            Apply(new BuildBindingsAdded(bindingsWithEmptyBuilds, CreateMetadata(userId: actionUserId)));
        }

        /// <summary>
        /// Удаляет привязки.
        /// </summary>
        /// <param name="buildsBinding">Привязка к билдам для удаления.</param>
        /// <param name="actionUserId">Идентификатор пользователя, кто добавил привязки.</param>
        public void RemoveBuildBinding(IEnumerable<BuildsBinding> buildsBinding, string actionUserId)
        {
            var bindingsToRemove = 
                BindingToBuilds
                    .Where(x => buildsBinding.Contains(x.Key))
                    .ToDictionary(x => x.Key, x => x.Value.ToArray());
            
            if (bindingsToRemove.IsEmpty())
            {
                return;
            }
            
            Apply(new BuildBindingsRemoved(bindingsToRemove, CreateMetadata(userId: actionUserId)));
        }

        /// <summary>
        /// Удаляет новые сборки в привязку.
        /// </summary>
        /// <param name="buildBinding">Привязка к сборкам.</param>
        /// <param name="buildIds">Идентификаторы новых сборок.</param>
        public void AddBuildsToBinding(BuildsBinding buildBinding, Guid[] buildIds)
        {
            if (!BindingToBuilds.ContainsKey(buildBinding))
            {
                return;
            }

            var newBuildIds = buildIds.Distinct().Except(BindingToBuilds[buildBinding]);
            if (newBuildIds.IsEmpty())
            {
                return;
            }

            Apply(new BuildsToBuildsBindingAdded(buildBinding, newBuildIds.ToArray(), CreateMetadata()));
        }

        /// <summary>
        /// Удаляет сборки из привязки.
        /// </summary>
        /// <param name="buildBinding">Привязка к сборкам.</param>
        /// <param name="buildIds">Идентификаторы сборок.</param>
        public void RemoveBuildsFromBinding(BuildsBinding buildBinding, Guid[] buildIds)
        {
            if (!BindingToBuilds.ContainsKey(buildBinding))
            {
                return;
            }

            var buildsToRemove = buildIds.Distinct().Intersect(BindingToBuilds[buildBinding]);
            if (buildsToRemove.IsEmpty())
            {
                return;
            }

            Apply(new BuildFromBuildsBindingRemoved(buildBinding, buildsToRemove.ToArray(), CreateMetadata()));
        }
        
        private void When(BuildBindingsAdded @event)
        {
            foreach ((BuildsBinding binding, Guid[] builds) in @event.BindingToBuilds)
            {
                BindingToBuilds.Add(binding, builds.ToList());
            }
        }

        private void When(BuildBindingsRemoved @event)
        {
            foreach ((BuildsBinding binding, Guid[] _) in @event.BindingsToBuilds)
            {
                BindingToBuilds.Remove(binding);
            }
        }

        private void When(BuildsToBuildsBindingAdded @event)
        {
            (BuildsBinding buildsBinding, Guid[] buildIds, _) = @event;
            
            BindingToBuilds[buildsBinding].AddRange(buildIds);
        }

        private void When(BuildFromBuildsBindingRemoved @event)
        {
            (BuildsBinding buildsBinding, Guid[] buildIds, _) = @event;
            
            BindingToBuilds[buildsBinding].RemoveAll(buildIds.Contains);
        }
    }
}
