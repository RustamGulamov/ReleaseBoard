using System;

namespace ReleaseBoard.Domain.Core.Exceptions
{
    /// <summary>
    /// Базовый класс для исключений при валидации.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <inheritdoc cref="Exception" />
        public ValidationException(object validationError)
        {
            ValidationError = validationError;
        }
        
        /// <inheritdoc cref="Exception" />
        public ValidationException(string msg) : base(msg)
        {
        }

        /// <inheritdoc cref="Exception" />
        public ValidationException(string msg, Exception exception) : base(msg, exception)
        {
        }
        
        /// <summary>
        /// Объект ошибки валидации.
        /// </summary>
        public object ValidationError { get; }
    }
}
