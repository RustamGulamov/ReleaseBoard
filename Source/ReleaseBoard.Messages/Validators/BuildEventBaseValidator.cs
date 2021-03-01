using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;
using ReleaseBoard.Common.Contracts.Extensions;
using ReleaseBoard.ReadModels;

namespace ReleaseBoard.Messages.Validators
{
    /// <summary>
    /// Базовый класс для валидаторов.
    /// </summary>
    /// <typeparam name="TEvent">Тип события.</typeparam>
    public class BuildEventBaseValidator<TEvent> : AbstractValidator<TEvent>
    {
        /// <summary>
        /// Добавляет ошибку в контекст, если существуют другие сборки с таким номером и суффиксом. 
        /// </summary>
        /// <param name="existedBuilds">Список существующих билдлов.</param>
        /// <param name="context"><see cref="CustomContext"/>.</param>
        /// <returns><see cref="Task"/>.</returns>
        protected void AddExistBuildsValidationFailure(IEnumerable<BuildReadModel> existedBuilds, CustomContext context)
        {
            var errorMessage = "Builds already exists. Exists builds: " +
                existedBuilds
                    .Select(x => x.ToString())
                    .JoinWith(";");
                
            context.AddFailure(new ValidationFailure(typeof(TEvent).Name, errorMessage)
            {
                ErrorCode = ValidatorErrorCodes.ExistSameBuild
            });
        }
    }
}
