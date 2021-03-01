using System;

namespace ReleaseBoard.Domain.Core.Exceptions
{
    /// <summary>
    /// Доменное исключение.
    /// </summary>
    public class DomainException : ApplicationException
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="msg">Сообщение.</param>
        public DomainException(string msg) : base(msg)
        {
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="msg">Сообщение.</param>
        /// <param name="exception"><see cref="Exception"/>.</param>
        public DomainException(string msg, Exception exception) : base(msg, exception)
        {
        }
    }
}
