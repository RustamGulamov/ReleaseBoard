using System;

namespace ReleaseBoard.Domain.EventStore.Exceptions
{
    /// <summary>
    /// Исключение возникает, когда версия не совпадает с версией потока.
    /// </summary>
    public class ChangesetConcurrencyException : Exception
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="expectedVersion">Ожидаемая версия.</param>
        /// <param name="actualVersion">Актуальная версия.</param>
        /// <param name="name">Название набор изменений.</param>
        public ChangesetConcurrencyException(long expectedVersion, long actualVersion, string name)
            : base($"Expected version {expectedVersion} in stream '{name}' but got {actualVersion}")
        {
            Name = name;
            ExpectedVersion = expectedVersion;
            ActualVersion = actualVersion;
        }

        /// <summary>
        /// Ожидаемая версия.
        /// </summary>
        public long ExpectedVersion { get; }

        /// <summary>
        /// Актуальная версия.
        /// </summary>
        public long ActualVersion { get; }

        /// <summary>
        /// Название набор изменений.
        /// </summary>
        public string Name { get; }
    }
}
