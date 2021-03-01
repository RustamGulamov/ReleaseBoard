using System;
using System.Collections.Generic;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.Infrastructure.Data.MongoDb.EventStore
{
    /// <summary>
    /// Модель набора изменений.
    /// </summary>
    public class Changeset : Entity
    {
        /// <summary>
        /// Название набора изменений.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Версия набора изменений.
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// Метаданные.
        /// </summary>
        public IMetadata Metadata { get; set; } = new Metadata();

        /// <summary>
        /// Данные набора изменений.
        /// </summary>
        public IReadOnlyCollection<Event> Data { get; set; } = new List<Event>();
    }
}
