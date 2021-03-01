using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.EventStore.Exceptions;

namespace ReleaseBoard.Infrastructure.Data.MongoDb.EventStore
{
    /// <summary>
    /// Реализация интерфейса <see cref="IChangesetRepository"/> для Монго.
    /// </summary>
    public class MongoChangesetRepository : IChangesetRepository
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="context">Контекст подключения.</param>
        public MongoChangesetRepository(IMongoContext context)
        {
            Collection = context.GetCollection<Changeset>("EventChangesets");
        }

        /// <summary>
        /// <see cref="IMongoCollection{Changeset}"/>.
        /// </summary>
        private IMongoCollection<Changeset> Collection { get; }

        /// <inheritdoc />
        public async Task Append(string name, IReadOnlyCollection<Event> events, long expectedVersion = -1)
        {
            long maxVersion =
                (await Collection.Find(x => x.Name == name)
                    .SortByDescending(x => x.Version)
                    .FirstOrDefaultAsync())?
                .Version ?? 0;

            if (expectedVersion >= 0 && expectedVersion != maxVersion)
            {
                throw new ChangesetConcurrencyException(expectedVersion, maxVersion, name);
            }
            
            var changeset = new Changeset
            {
                Name = name, 
                Version = maxVersion + 1, 
                Data = events,
                Metadata = new Metadata()
                {
                    Timestamp = DateTimeOffset.Now
                }
            };

            await Collection.InsertOneAsync(changeset);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventsWithVersion>> Read(string name, long afterVersion, int maxCount)
        {
            List<Changeset> changesets = 
                await Collection
                    .Find(x => x.Name == name && x.Version > afterVersion)
                    .Limit(maxCount)
                    .ToListAsync();

            return changesets.Select(x => new EventsWithVersion(x.Version, x.Data));
        }
    }
}
