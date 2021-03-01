using System;

namespace ReleaseBoard.Web.ApiModels.DistributionModels
{
    /// <summary>
    /// Модель привязки к сборкам.
    /// </summary>
    public class BuildBindingModel : IEquatable<BuildBindingModel>
    {
        /// <summary>
        /// Путь к каталогу с билдами.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Паттерн для распознавания сборки.
        /// </summary>
        public Guid PatternId { get; set; }

        /// <summary>
        /// Тип хранилища.
        /// </summary>
        public BuildSourceType SourceType { get; set; }

        /// <inheritdoc />
        public bool Equals(BuildBindingModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return
                string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase) && 
                PatternId.Equals(other.PatternId) && 
                SourceType == other.SourceType;
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

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((BuildBindingModel)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ PatternId.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)SourceType;
                return hashCode;
            }
        }
    }
}
