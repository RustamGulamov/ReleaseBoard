using System;
using System.Collections.Generic;
using System.Linq;
using ReleaseBoard.Domain.Core.Exceptions;

namespace ReleaseBoard.Domain.Core
{
    /// <summary>
    /// Класс метаданные.
    /// </summary>
    public class Metadata : Dictionary<string, string>, IMetadata
    {
        /// <summary>
        /// Пустой метаданные.
        /// </summary>
        public static IMetadata Empty = new Metadata();

        /// <summary>
        /// Конструктор.
        /// </summary>
        public Metadata()
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="keyValuePairs">a.</param>
        public Metadata(IDictionary<string, string> keyValuePairs)
            : base(keyValuePairs)
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="keyValuePairs">a.</param>
        public Metadata(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
            : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        /// <inheritdoc />
        public Guid AggregateId
        {
            get => GetMetadataValue(nameof(AggregateId), Guid.Parse);
            set => Add(nameof(AggregateId), value.ToString());
        }

        /// <inheritdoc />
        public DateTimeOffset Timestamp
        {
            get => GetMetadataValue(nameof(Timestamp), DateTimeOffset.Parse);
            set => Add(nameof(Timestamp), value.ToString("O"));
        }

        /// <inheritdoc />
        public string UserId
        {
            get => GetMetadataValue(nameof(UserId));
            set => Add(nameof(UserId), value);
        }

        /// <inheritdoc />
        public override string ToString() => 
            string.Join(Environment.NewLine, this.Select(kv => $"{kv.Key}: {kv.Value}"));

        private string GetMetadataValue(string key) => 
            GetMetadataValue(key, s => s);

        private T GetMetadataValue<T>(string key, Func<string, T> converter)
        {
            if (!TryGetValue(key, out var value))
            {
                throw new MetadataKeyNotFoundException(key);
            }

            try
            {
                return converter(value);
            }
            catch (Exception e)
            {
                throw new MetadataParseException(key, value, e);
            }
        }
    }
}
