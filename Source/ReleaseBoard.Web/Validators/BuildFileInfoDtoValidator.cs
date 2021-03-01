using System;
using FluentValidation;
using ReleaseBoard.Web.ApiModels.BuildModels;

namespace ReleaseBoard.Web.Validators
{
    /// <summary>
    /// Валидатор для массива информации о загружаемых файлах.
    /// </summary>
    public class BuildFileInfoDtoValidator : AbstractValidator<BuildFileInfoDto[]>
    {
        /// <summary>
        /// .ctor.
        /// </summary>
        public BuildFileInfoDtoValidator()
        {
            RuleFor(x => x).NotEmpty().WithMessage("Сборка должна содержать минимум 1 файл");
            RuleForEach(x => x)
                .NotNull()
                .WithMessage("Информация о файлах не должна содержать null");
        }
    }
}
