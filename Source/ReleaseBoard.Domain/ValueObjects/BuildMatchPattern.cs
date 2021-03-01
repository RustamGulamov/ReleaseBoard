using System;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.Core.Extensions;
using ReleaseBoard.Domain.ValueObjects.Exceptions;

namespace ReleaseBoard.Domain.ValueObjects
{
    /// <summary>
    /// Паттерн для распознавания сборок.
    /// </summary>
    public class BuildMatchPattern : ValueObject<BuildMatchPattern>
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="regexp">Регулярное выражение.</param>
        /// <param name="title">Заголовок.</param>
        public BuildMatchPattern(string regexp, string title = null)
        {
            regexp.ThrowIsNullOrWhiteSpace(new CreateValueObjectException(nameof(BuildMatchPattern), "Regular expression not defined"));

            Title = title;
            Regexp = regexp;
        }

        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Регулярное выражение.
        /// </summary>
        public string Regexp { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Title ?? Regexp}";
        }

        /// <inheritdoc />
        protected override bool EqualsCore(BuildMatchPattern other)
        {
            return Regexp.Equals(other.Regexp);
        }

        /// <inheritdoc />
        protected override int GetHashCodeCore()
        {
            return HashCode.Combine(Regexp);
        }
    }
}
