using System;

namespace ReleaseBoard.Domain.Core.Exceptions
{
    /// <summary>
    /// Исключение возникает, когда ключ отсутствует в метаданные.
    /// </summary>
    public class MetadataKeyNotFoundException : ArgumentOutOfRangeException
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="key">Ключ.</param>
        public MetadataKeyNotFoundException(string key)
            : base($"Could not find metadata key '{key}'")
        {
        }
    }
}
