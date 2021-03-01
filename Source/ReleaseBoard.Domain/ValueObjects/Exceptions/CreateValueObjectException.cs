using System;
using ReleaseBoard.Domain.Core.Exceptions;

namespace ReleaseBoard.Domain.ValueObjects.Exceptions
{
    /// <summary>
    /// Исключение при ошибке создания объекта значения.
    /// </summary>
    public class CreateValueObjectException : DomainException
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="msg">Сообщение.</param>
        public CreateValueObjectException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="objectName">Название value object.</param>
        /// <param name="msg">Сообщение.</param>
        public CreateValueObjectException(string objectName , string msg) : base($"Cannot create value object {objectName}: {msg}")
        {
        }
    }
}
