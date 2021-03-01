using System;
using System.Collections.Generic;

namespace ReleaseBoard.Application.CommandHandlers.Builds.ChangeState
{
    /// <summary>
    /// Объект ошибки валидации при переименовании билда.
    /// </summary>
    public class BuildTransferRuleCheckValidationError : BuildStateChangeRequestValidationError
    {
        /// <summary>
        /// Имена правил, не соответствующих условию.
        /// </summary>
        public IEnumerable<string> Rules { get; init; }
    }
}
