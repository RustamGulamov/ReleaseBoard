using System;

namespace ReleaseBoard.Domain.Core
{
    /// <summary>
    /// Снепшот для агрегата.
    /// </summary>
    public class AggregateSnapshot
    {
        /// <summary>
        /// Идентификатор агрегата.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор агрегата.
        /// </summary>
        public long Version { get; set; }
    }
}
