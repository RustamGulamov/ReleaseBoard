using System;
using ReleaseBoard.Domain.Core.Exceptions;

namespace ReleaseBoard.Domain.Distributions.Exceptions
{
    /// <summary>
    /// Исключение, при создании дистрибутива.
    /// </summary>
    public class CreateDistributionException : DomainException
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="msg">Сообщение.</param>
        public CreateDistributionException(string msg) 
            : base($"Invalid create distribution operation because : {msg}")
        {
        }
    }
}
