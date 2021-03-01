using System;
using System.Collections.Generic;

namespace ReleaseBoard.Domain.Core
{
    /// <summary>
    /// Интерфейс метаданные.
    /// </summary>
    public interface IMetadata : IDictionary<string, string>
    {
        /// <summary>
        /// Идентификатор агрегата.
        /// </summary>
        public Guid AggregateId { get; }

        /// <summary>
        /// Timestamp.
        /// </summary>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        string UserId { get; }
    }
}
