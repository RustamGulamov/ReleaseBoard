using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ReleaseBoard.Application.Interfaces;
using ReleaseBoard.Web.ApiModels.DistributionModels;

namespace ReleaseBoard.Web.Validators.ModelValidators
{
    /// <inheritdoc />
    public class ProjectBindingModelValidator : AbstractValidator<ProjectBindingModel>
    {
        private readonly ILighthouseServiceApi lighthouseService;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="lighthouseService"><see cref="ILighthouseServiceApi"/>.</param>
        public ProjectBindingModelValidator(ILighthouseServiceApi lighthouseService)
        {
            this.lighthouseService = lighthouseService;

            RuleFor(b => b.ProjectId)
                .MustAsync(IsValid)
                .WithMessage("Невалидный проект.");

            RuleFor(b => b.MaskProjectVersion)
                .NotEmpty()
                .WithMessage("Маска версии проекта не должна быть пустой.");
        }

        private async Task<bool> IsValid(Guid projectId, CancellationToken token) => 
            await lighthouseService.GetProjectById(projectId) != null;
    }
}
