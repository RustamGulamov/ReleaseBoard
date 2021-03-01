using System;
using ReleaseBoard.Domain.Core.Exceptions;

namespace ReleaseBoard.Domain.Builds.Exceptions
{
    /// <summary>
    /// Исключение при ошибке обновления сборки.
    /// </summary>
    public class UpdateBuildException : DomainException
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="msg">Сообщение об ошибке.</param>
        public UpdateBuildException(string msg)
            : base($"Invalid update build operation because : {msg}")
        {
        }
    }
}
