using System;
using ReleaseBoard.Domain.Core.Exceptions;

namespace ReleaseBoard.Domain.Distributions.Exceptions
{
    /// <summary>
    /// Исключение при пустом имени объекта.
    /// </summary>
    public class UpdateDistributionException : DomainException
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="msg">Сообщение.</param>
        public UpdateDistributionException(string msg) 
            : base($"Invalid update distribution operation because : {msg}")
        {
        }
    }
}
