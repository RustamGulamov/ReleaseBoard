using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.ReadModels;
using ReleaseBoard.Web.ApiModels.BuildModels;

namespace ReleaseBoard.Web.Validators
{
    /// <inheritdoc />
    public class GenerateNewBuildPathsRequestValidator : AbstractValidator<GenerateNewBuildPathsRequestModel>
    {
        private readonly IReadOnlyRepository<DistributionReadModel> distributionRepository;
        private readonly IReadOnlyRepository<BuildReadModel> buildRepository;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="distributionRepository"><see cref="IReadOnlyRepository{DistributionReadModel}"/>.</param>
        /// <param name="buildRepository"><see cref="IReadOnlyRepository{BuildReadModel}"/>.</param>
        public GenerateNewBuildPathsRequestValidator(
            IReadOnlyRepository<DistributionReadModel> distributionRepository, 
            IReadOnlyRepository<BuildReadModel> buildRepository)
        {
            this.distributionRepository = distributionRepository;
            this.buildRepository = buildRepository;
            CascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Number).NotNull().WithMessage("Номер версии не должен быть пустым");

            RuleFor(x => x.DistributionId)
                .MustAsync(IsValid)
                .WithMessage(x => $"Дистрибутив c идентификатором {x.DistributionId} не существует");

            RuleFor(x => x.Number)
                .Must(IsValid)
                .WithMessage("Номер сборки невалидный.");

            RuleFor(x => x)
                .MustAsync(IsUniqueBuildNumber)
                .WithMessage(model => $"Номер сборки {model.Number} уже существует.");
        }

        private async Task<bool> IsValid(Guid distributionId, CancellationToken token) =>
            await distributionRepository.Any(x => x.Id == distributionId, token);

        private bool IsValid(string versionNumber) =>
            Version.TryParse(versionNumber, out Version version) &&
            version.Major >= 0 && version.Minor >= 0 && version.Build >= 0;

        private async Task<bool> IsUniqueBuildNumber(GenerateNewBuildPathsRequestModel model, CancellationToken token) =>
            !await buildRepository.Any(x => x.DistributionId == model.DistributionId && x.Number == model.Number, token);
    }
}
