using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Domain.Core.Interfaces;

namespace ReleaseBoard.Infrastructure.Data.MongoDb
{
    /// <summary>
    /// Mongo generic repository.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности.</typeparam>
    public class MongoRepository<TEntity> : IRepository<TEntity>, ISearchRepository<TEntity>
    {
        private FindOptions options = null;
        
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="context">Контекст подключения.</param>
        public MongoRepository(IMongoContext context)
        {
            Collection = context.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        /// <summary>
        /// <see cref="IMongoCollection{TEntity}"/>.
        /// </summary>
        protected IMongoCollection<TEntity> Collection { get; }

        /// <inheritdoc />
        public virtual Task Add(TEntity entity, CancellationToken cancellationToken = default)
        {
            return Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public Task AddMany(TEntity[] entities, CancellationToken cancellationToken = default)
        {
            return Collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
        }
        
        /// <inheritdoc />
        public virtual async Task<bool> Update(Expression<Func<TEntity, bool>> filter, TEntity entity, CancellationToken cancellationToken = default)
        {
            ReplaceOneResult result = await Collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        /// <inheritdoc />
        public virtual async Task<bool> Delete(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            DeleteResult result = await Collection.DeleteOneAsync(filter, cancellationToken);
            return result.DeletedCount > 0;
        }
        
        /// <inheritdoc />
        public virtual async Task<TEntity> Query(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) => 
            await Collection.Find(filter, options).FirstOrDefaultAsync(cancellationToken);

        /// <inheritdoc />
        public virtual Task<IList<TEntity>> GetAll(CancellationToken cancellationToken = default) => 
            QueryAll(x => true, cancellationToken);

        /// <inheritdoc />
        public virtual async Task<IList<TEntity>> QueryAll(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) => 
            await Collection.Find(filter, options).ToListAsync(cancellationToken);

        /// <inheritdoc />
        public virtual async Task<IList<TProjection>> ProjectManyAsync<TProjection>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProjection>> projection, CancellationToken cancellationToken = default)
            where TProjection : new()
        {
            return await Collection.Find(filter).Project(projection).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TProjection> ProjectOne<TProjection>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TProjection>> projection, CancellationToken cancellationToken = default)
            where TProjection : class, new()
        {
            return await Collection.Find(filter).Project(projection).FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<bool> Any(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) => 
            await Collection.Find(filter, options).AnyAsync(cancellationToken);

        /// <inheritdoc />
        public virtual async Task CreateIndex(params Expression<Func<TEntity, object>>[] fields)
        {
            await Collection.Indexes.CreateManyAsync(
                fields
                    .SelectMany(i => new[]
                    {
                        Builders<TEntity>.IndexKeys.Ascending(i),
                    })
                    .Select(i => new CreateIndexModel<TEntity>(i))
            );
        }
        
        /// <inheritdoc />
        public Task<List<TEntity>> TextSearch(string value, Expression<Func<TEntity, bool>> filter = null, params Expression<Func<TEntity, object>>[] fields)
        {
            BsonRegularExpression regex = 
                BsonRegularExpression.Create(new Regex(Regex.Escape(value), RegexOptions.IgnoreCase));

            FilterDefinition<TEntity> filterDefinition = 
                Builders<TEntity>.Filter.Or(fields.Select(field => Builders<TEntity>.Filter.Regex(field, regex)));
            
            if (filter != null)
            {
                filterDefinition = Builders<TEntity>.Filter.And(filter, filterDefinition);
            }
            
            return Collection.Find(filterDefinition).ToListAsync();
        }
    }
}
