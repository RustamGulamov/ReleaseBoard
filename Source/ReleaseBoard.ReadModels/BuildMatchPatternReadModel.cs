using System;
using ReleaseBoard.Domain.Core;

namespace ReleaseBoard.ReadModels
{
    /// <summary>
    /// Паттерн для распознавания сборок.
    /// </summary>
    public class BuildMatchPatternReadModel : Entity, IEquatable<BuildMatchPatternReadModel>
    {
        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Краткое описание, пример подходящей строки.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Регулярное выражение.
        /// </summary>
        public string Regexp { get; set; }

        /// <inheritdoc />
        public bool Equals(BuildMatchPatternReadModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Title == other.Title && Description == other.Description && Regexp == other.Regexp;
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

            return Equals((BuildMatchPatternReadModel)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() => 
            HashCode.Combine(Regexp, Description, Title, Id);
    }
}
