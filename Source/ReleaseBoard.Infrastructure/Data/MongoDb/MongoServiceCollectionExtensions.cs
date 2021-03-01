using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Domain.Builds.Events;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.Domain.Distributions.Events;
using ReleaseBoard.Domain.Distributions.Events.BuildBindings;
using ReleaseBoard.Domain.Distributions.Events.LifeCycleStates;
using ReleaseBoard.Domain.Distributions.Events.Owners;
using ReleaseBoard.Domain.Distributions.Events.ProjectBindings;
using ReleaseBoard.Domain.EventStore;
using ReleaseBoard.Domain.ValueObjects;
using ReleaseBoard.Infrastructure.Data.MongoDb.EventStore;

namespace ReleaseBoard.Infrastructure.Data.MongoDb
{
    /// <summary>
    /// Extension методы <see cref="IServiceCollection"/> для Монго.
    /// </summary>
    public static class MongoServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрирует конфиги Монго БД.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>.</param>
        /// <param name="connectionString">Connection string.</param>
        internal static void RegisterMongoDB(this IServiceCollection services, string connectionString)
        {
            RegisterEventsMap(); 
            RegisterSerializer();

            services.AddScoped(typeof(IReadOnlyRepository<>), typeof(MongoRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));
            services.AddScoped(typeof(ISearchRepository<>), typeof(MongoRepository<>));

            services.AddSingleton<IMongoContext, MongoContext>(provider => new MongoContext(connectionString));
            services.AddScoped<IChangesetRepository, MongoChangesetRepository>();
            // TODO: snapshot repository.
        }
        
        private static void RegisterSerializer()
        {
            BsonSerializer.RegisterSerializer(typeof(IMetadata), new MetadataSerializer());
        }
        
        private class MetadataSerializer : DictionaryInterfaceImplementerSerializer<Dictionary<string, string>>
        {
            public override Dictionary<string, string> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            {
                return new Metadata(base.Deserialize(context, args));
            }
        }
    }
}
