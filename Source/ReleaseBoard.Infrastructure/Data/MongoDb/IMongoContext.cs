using System;
using MongoDB.Driver;

namespace ReleaseBoard.Infrastructure.Data.MongoDb
{
    /// <summary>
    /// Интерфейс контекста подключения к Mongo.
    /// </summary>
    public interface IMongoContext
    {
        /// <summary>
        /// Возвращает коллекцию по названию.
        /// </summary>
        /// <param name="name">Название коллекции.</param>
        /// <returns>Коллекция типа T.</returns>
        IMongoCollection<T> GetCollection<T>(string name);
    }
}