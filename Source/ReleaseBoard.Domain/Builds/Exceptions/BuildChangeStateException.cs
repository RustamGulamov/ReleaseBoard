using System;
using ReleaseBoard.Domain.Core.Exceptions;

namespace ReleaseBoard.Domain.Builds.Exceptions
{
    /// <summary>
    /// Исключение при ошибке изменения state у сборки.
    /// </summary>
    public class BuildChangeStateException : DomainException
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="msg">Сообщение с ошибкой.</param>
        /// <param name="exception">Внутреннее исключение.</param>
        public BuildChangeStateException(string msg, Exception exception) : base(msg, exception)
        {
        }
    }
}
