using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ReleaseBoard.Domain.Core.Interfaces;
using ReleaseBoard.ReadModels;
using ReleaseBoard.Web.ApiModels.DistributionModels;

namespace ReleaseBoard.Web.Validators.ModelValidators
{
    /// <inheritdoc />
    public class BuildBindingModelValidator : AbstractValidator<BuildBindingModel>
    {
        private const string RelativePathValidation = @"^[^./\\:*?<>|]([^:*?<>|]+\\?)*$";
        private readonly IReadOnlyRepository<BuildMatchPatternReadModel> buildMatchPatternRepository;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="buildMatchPatternRepository">Сервис для работы с объектами <see cref="IReadOnlyRepository{BuildMatchPatternReadModel}"/>.</param>
        public BuildBindingModelValidator(
            IReadOnlyRepository<BuildMatchPatternReadModel> buildMatchPatternRepository)
        {
            this.buildMatchPatternRepository = buildMatchPatternRepository;
            CascadeMode = CascadeMode.Stop;

            RuleFor(b => b.SourceType)
                .IsInEnum()
                .DependentRules(() =>
                {
                    RuleFor(b => b.Path)
                        .NotEmpty()
                            .WithMessage("Путь не должен быть пустым")
                        .MaximumLength(256)
                            .WithMessage("Путь должен быть меньше 256 символов")
                        .Matches(RelativePathValidation)
                            .WithMessage("Путь содержит недопустимые символы")
                        .DependentRules(() =>
                        {
                            RuleFor(b => b.PatternId).MustAsync(IsValidBuildMatchPatternAsync)
                                .WithMessage("Невалидный паттерн распознающий сборки");
                        });
                });
        }

        private async Task<bool> IsValidBuildMatchPatternAsync(Guid patternId, CancellationToken token) =>
            await buildMatchPatternRepository.Any(x => x.Id == patternId, token);
    }
}
