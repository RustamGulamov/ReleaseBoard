using System;
using ReleaseBoard.Common.Contracts.Abstractions;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Extensions;
using ReleaseBoard.Domain.ValueObjects.Exceptions;

namespace ReleaseBoard.Domain.ValueObjects
{
    /// <summary>
    /// Привязка к сборкам в хранилище.
    /// </summary>
    public class BuildsBinding : ValueObject<BuildsBinding>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="path">Путь к сборкам.</param>
        /// <param name="pattern">Паттерн.</param>
        /// <param name="sourceType">Тип источника.</param>
        public BuildsBinding(string path, BuildMatchPattern pattern, BuildSourceType sourceType)
        {
            path.ThrowIsNullOrWhiteSpace(new CreateValueObjectException(nameof(BuildsBinding), "Invalid path"));
            pattern.ThrowIfNull(new CreateValueObjectException(nameof(BuildsBinding), "Invalid pattern"));

            Path = path;
            Pattern = pattern;
            SourceType = sourceType;
        }

        /// <summary>
        /// Путь к каталогу с билдами.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Паттерн для распознавания сборки.
        /// </summary>
        public BuildMatchPattern Pattern { get; }

        /// <summary>
        /// Тип хранилища.
        /// </summary>
        public BuildSourceType SourceType { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Path: {Path}, SourceType: {SourceType}, Pattern: {Pattern}";
        }

        /// <inheritdoc />
        protected override bool EqualsCore(BuildsBinding other)
        {
            return Path == other.Path && SourceType == other.SourceType && other.Pattern.Equals(Pattern);
        }

        /// <inheritdoc />
        protected override int GetHashCodeCore() =>
            HashCode.Combine(Path, Pattern, SourceType);
    }
}
