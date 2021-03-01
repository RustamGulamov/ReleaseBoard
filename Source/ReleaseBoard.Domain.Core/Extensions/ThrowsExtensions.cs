using System;
using ReleaseBoard.Domain.Core.Exceptions;

namespace ReleaseBoard.Domain.Core.Extensions
{
    /// <summary>
    /// Расширения для исключений.
    /// </summary>
    public static class ThrowsExtensions
    {
        /// <summary>
        /// Бросает exception нужно типа, если строка пустая или null.
        /// </summary>
        /// <param name="obj">Строка.</param>
        /// <param name="exception">Исключение для выбрасывания.</param>
        public static void ThrowIsNullOrWhiteSpace(this string obj, DomainException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (string.IsNullOrWhiteSpace(obj))
                throw exception;
        }

        /// <summary>
        /// Бросает exception нужно типа, если строка пустая или null.
        /// </summary>
        /// <param name="obj">Объект.</param>
        /// <param name="exception">Исключение для выбрасывания.</param>
        public static void ThrowIfNull(this object obj, DomainException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (obj == null)
                throw exception;
        }
    }
}
