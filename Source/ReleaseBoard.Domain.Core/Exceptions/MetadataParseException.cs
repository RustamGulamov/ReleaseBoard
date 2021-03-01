using System;

namespace ReleaseBoard.Domain.Core.Exceptions
{
    /// <summary>
    /// Исключение возникает, если парсить не получиться.
    /// </summary>
    public class MetadataParseException : Exception
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <param name="value">Значение.</param>
        /// <param name="innerException">Exception.</param>
        public MetadataParseException(string key, string value, Exception innerException)
            : base($"Failed to parse metadata key '{key}' with value '{value}' due to '{innerException.Message}'", innerException)
        {
        }
    }
}
