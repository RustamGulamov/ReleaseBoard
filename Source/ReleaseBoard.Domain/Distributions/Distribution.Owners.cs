using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Common.Contracts.Extensions;
using ReleaseBoard.Domain.Distributions.Events.Owners;
using ReleaseBoard.Domain.ValueObjects;

namespace ReleaseBoard.Domain.Distributions
{
    /// <summary>
    /// Дистрибутив. Методы изменения ответственных.
    /// </summary>
    public partial class Distribution
    {
        /// <summary>
        /// Добавляет новых ответственных дистрибутива.
        /// </summary>
        /// <param name="owners">Список новых ответственных дистрибутива для добавления.</param>
        public void AddOwners(IList<User> owners)
        {
            IList<User> newOwners = owners.Except(Owners).ToList();
            if (newOwners.IsEmpty())
            {
                return;
            }

            Apply(new DistributionOwnersAdded(newOwners, CreateMetadata()));
        }

        /// <summary>
        /// Удаляет старых ответственных дистрибутива.
        /// </summary>
        /// <param name="owners">Список старых ответственных дистрибутива для удаления.</param>
        public void RemoveOwners(IList<User> owners)
        {
            IList<User> ownersToRemove = owners.Intersect(Owners).ToList();
            if (ownersToRemove.IsEmpty())
            {
                return;
            }

            Apply(new DistributionOwnersRemoved(owners, CreateMetadata()));
        }

        private void When(DistributionOwnersAdded @event) =>
                Owners.AddRange(@event.Owners);

        private void When(DistributionOwnersRemoved @event) =>
            Owners.RemoveAll(@event.Owners.Contains);
    }
}
