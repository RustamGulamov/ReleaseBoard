using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.EventStore.Exceptions;
using ReleaseBoard.Infrastructure.Data.MongoDb.EventStore;

namespace ReleaseBoard.UnitTests.Domain.EventStores
{
    /// <inheritdoc/>
    internal class MemoryChangesetRepository : IChangesetRepository
    {
        /// <summary>
        /// Список событий.
        /// </summary>
        public readonly ICollection<Changeset> EventsStorage = new List<Changeset>();

        /// <inheritdoc />
        public Task Append(string name, IReadOnlyCollection<Event> events, long expectedVersion = -1)
        {
            var eventsFormStorage = EventsStorage.Where(x => x.Name == name);

            long maxVersion = eventsFormStorage.Any() ? eventsFormStorage.Select(x => x.Version).Max() : 0;

            if (expectedVersion >= 0 && expectedVersion != maxVersion)
            {
                throw new ChangesetConcurrencyException(expectedVersion, maxVersion, name);
            }

            EventsStorage.Add(new Changeset { Name = name, Version = maxVersion + 1, Data = events.ToList() });

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventsWithVersion>> Read(string name, long afterVersion, int maxCount)
        {
            await Task.CompletedTask;

            return EventsStorage
                .Where(x => x.Name == name && x.Version > afterVersion)
                .OrderBy(x => x.Version)
                .Take(maxCount)
                .Select(x => new EventsWithVersion(x.Version, x.Data));
        }
    }
}
