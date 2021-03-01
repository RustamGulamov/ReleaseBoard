using System;

namespace ReleaseBoard.Domain.Core
{
    /// <summary>
    /// Базовый модель сущности.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Идентификатор сущности.
        /// </summary>
        public Guid Id { get; set; }
    }
}
