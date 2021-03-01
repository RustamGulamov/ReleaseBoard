using System;
using ReleaseBoard.Domain.Core;
using ReleaseBoard.Domain.ValueObjects.Exceptions;

namespace ReleaseBoard.Domain.ValueObjects
{
    /// <summary>
    /// Представляет номер сборки.
    /// </summary>
    public class VersionNumber : ValueObject<VersionNumber>
    {
        private readonly Version version;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="numberString">Номер сборки в виде строки.</param>
        public VersionNumber(string numberString)
        {
            if (!Version.TryParse(numberString, out version))
            {
                throw new CreateValueObjectException(nameof(VersionNumber), "Invalid numberString string");
            }
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="version"><see cref="Version"/>.</param>
        private VersionNumber(Version version)
        {
            this.version = version;
        }

        /// <summary>
        /// Попытаться парсить и создать объект <see cref="VersionNumber"/>.
        /// </summary>
        /// <param name="numberString">Номер сборки в виде строки.</param>
        /// <param name="versionNumber"><see cref="VersionNumber"/>.</param>
        /// <returns>Результат парсинга.</returns>
        public static bool TryParse(string numberString, out VersionNumber versionNumber)
        {
            versionNumber = null;
            if (Version.TryParse(numberString, out Version result))
            {
                versionNumber = new VersionNumber(result);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверяет что версия включена в другу.
        /// </summary>
        /// <param name="number">Версия для проверки.</param>
        /// <returns>Результат проверки.</returns>
        public bool IsInclude(VersionNumber number) =>
            number.ToString().StartsWith(version.ToString())
            && number.version >= version;

        /// <inheritdoc/>
        public override string ToString()
        {
            return version.ToString();
        }

        /// <inheritdoc />
        protected override bool EqualsCore(VersionNumber other)
        {
            return version.Equals(other.version);
        }

        /// <inheritdoc />
        protected override int GetHashCodeCore()
        {
            return (version != null ? version.GetHashCode() : 0);
        }
    }
}
