using System;
using ReleaseBoard.Common.Contracts.Abstractions;

namespace ReleaseBoard.ReadModels
{
    /// <summary>
    /// Модель привязки к сборкам.
    /// </summary>
    public class BuildBindingReadModel : IEquatable<BuildBindingReadModel>
    {
        /// <summary>
        /// Путь к каталогу с билдами.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Паттерн для распознавания сборки.
        /// </summary>
        public BuildMatchPatternReadModel Pattern { get; set; }

        /// <summary>
        /// Тип хранилища.
        /// </summary>
        public BuildSourceType SourceType { get; set; }

        /// <inheritdoc />
        public bool Equals(BuildBindingReadModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Path == other.Path && Equals(Pattern, other.Pattern) && SourceType == other.SourceType;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((BuildBindingReadModel)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => 
            HashCode.Combine(Pattern, Path, SourceType);

        /// <inheritdoc />
        public override string ToString() => 
            $"Path: {Path}, BuildSourceType: {SourceType}, Pattern: {Pattern?.Regexp}";
    }
}
