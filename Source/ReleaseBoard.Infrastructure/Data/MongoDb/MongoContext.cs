using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ReleaseBoard.Infrastructure.Data.MongoDb
{
    /// <summary>
    /// Контекст подключения к Mongo.
    /// </summary>
    public class MongoContext : IMongoContext
    {
        private readonly IMongoDatabase database;

        /// <summary>
        /// Конструктор, создает подключение к БД.
        /// </summary>
        /// <param name="connectionString">Строка подключения к Mongo.</param>
        public MongoContext(string connectionString)
        {
            MongoDefaults.GuidRepresentation = GuidRepresentation.Standard;

            MongoUrl url = MongoUrl.Create(connectionString);
            IMongoClient client = new MongoClient(url);
            database = client.GetDatabase(url.DatabaseName);
        }

        /// <inheritdoc />
        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return database.GetCollection<T>(name);
        }
    }
}
