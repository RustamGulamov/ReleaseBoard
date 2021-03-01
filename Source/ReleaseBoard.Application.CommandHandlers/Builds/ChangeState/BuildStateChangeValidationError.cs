using System;
using ReleaseBoard.Domain.Builds;

namespace ReleaseBoard.Application.CommandHandlers.Builds.ChangeState
{
    /// <summary>
    /// Объект ошибки валидации при переименовании билда.
    /// </summary>
    public class BuildStateChangeRequestValidationError
    {
        /// <summary>
        /// Текущий статус сборки.
        /// </summary>
        public LifeCycleState From { get; init; }
        
        /// <summary>
        /// Новый статус.
        /// </summary>
        public LifeCycleState To { get; init; }
        
        /// <summary>
        /// Название ошибки.
        /// </summary>
        public string ErrorName { get; init; }
    }
}
