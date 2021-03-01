using System;
using ReleaseBoard.Domain.Core.Exceptions;

namespace ReleaseBoard.Domain.Builds.Exceptions
{
   /// <summary>
   /// Исключение при ошибке создания сборки.
   /// </summary>
    public class CreateBuildException : DomainException
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="msg">Сообщение об ошибке.</param>
        public CreateBuildException(string msg)
            : base($"Invalid create build operation because : {msg}")
        {
        }
    }
}
